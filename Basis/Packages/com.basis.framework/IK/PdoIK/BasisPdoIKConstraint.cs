using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	/// <summary>
	/// The Spine IK constraint data based on PDO-IK distance optimization.
	/// </summary>
	[System.Serializable]
	public struct BasisSpineIKConstraintData : IAnimationJobData, BasisISpineIKConstraintData
	{
		[SerializeField] Transform m_Hips;
		[SerializeField] Transform[] m_SpineJoints;
		[SerializeField] Transform m_Head;

		[SyncSceneToStream, SerializeField]
		public Vector3 headTargetPosition;
		[SyncSceneToStream, SerializeField]
		public Vector3 headTargetRotation;

		[SyncSceneToStream, SerializeField]
		public float ChainWeight;

		[SyncSceneToStream, SerializeField]
		public bool MaintainSpineLength;

		// Interface implementation using head-specific naming
		Vector3 BasisISpineIKConstraintData.headTargetPosition { get => headTargetPosition; }
		Vector3 BasisISpineIKConstraintData.headTargetRotation { get => headTargetRotation; }
		float BasisISpineIKConstraintData.chainWeight { get => ChainWeight; }

		public Transform hips { get => m_Hips; set => m_Hips = value; }
		public Transform[] spineJoints { get => m_SpineJoints; set => m_SpineJoints = value; }
		public Transform head { get => m_Head; set => m_Head = value; }
		public float chainWeight { get => ChainWeight; set => ChainWeight = value; }
		public bool maintainSpineLength { get => MaintainSpineLength; set => MaintainSpineLength = value; }

		string BasisISpineIKConstraintData.chainWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(ChainWeight));
		string BasisISpineIKConstraintData.headTargetPositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headTargetPosition));
		string BasisISpineIKConstraintData.headTargetRotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headTargetRotation));

		[SerializeField]
		public Vector3[] m_OriginalDistances;

		public Vector3[] originalDistances
		{
			get { return m_OriginalDistances; }
		}

		bool IAnimationJobData.IsValid() =>
			(m_Hips != null && m_Head != null && m_SpineJoints != null && m_SpineJoints.Length > 0 &&
			 IsValidSpineChain());

		private bool IsValidSpineChain()
		{
			if (m_SpineJoints == null || m_SpineJoints.Length == 0) return false;

			// Check distances are reasonable
			Transform current = m_Hips;
			for (int i = 0; i < m_SpineJoints.Length; i++)
			{
				if (m_SpineJoints[i] == null) return false;

				float distance = Vector3.Distance(current.position, m_SpineJoints[i].position);
				if (distance > 2.0f || distance < 0.01f) // Reasonable spine segment lengths
				{
					Debug.LogWarning($"Unusual spine segment length: {distance}m between {current.name} and {m_SpineJoints[i].name}");
				}

				current = m_SpineJoints[i];
			}

			float headDistance = Vector3.Distance(current.position, m_Head.position);
			if (headDistance > 1.0f || headDistance < 0.01f)
			{
				Debug.LogWarning($"Unusual head distance: {headDistance}m");
			}

			return true;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			m_Hips = null;
			m_SpineJoints = new Transform[0];
			m_Head = null;
			ChainWeight = 1.0f;
			MaintainSpineLength = true;

			// Initialize original distances array
			if (m_SpineJoints != null && m_SpineJoints.Length > 0)
			{
				CalibrateOriginalDistances();
			}
		}

		private void CalibrateOriginalDistances()
		{
			if (m_SpineJoints == null || m_SpineJoints.Length == 0) return;

			int jointCount = m_SpineJoints.Length + 1; // Include root
			m_OriginalDistances = new Vector3[jointCount];

			// Store original distances between consecutive joints
			m_OriginalDistances[0] = Vector3.zero; // Root has no previous joint

			Transform prev = m_Hips;
			for (int i = 0; i < m_SpineJoints.Length; i++)
			{
				if (m_SpineJoints[i] != null)
				{
					Vector3 offset = m_SpineJoints[i].position - prev.position;
					m_OriginalDistances[i + 1] = offset;
					prev = m_SpineJoints[i];
				}
			}
		}
	}

	[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Spine IK Constraint")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/SpineIKConstraint.html")]
	public class BasisSpineIKConstraint : RigConstraint<BasisSpineIKConstraintJob, BasisSpineIKConstraintData, BasisSpineIKConstraintJobBinder<BasisSpineIKConstraintData>>
	{
		/// <inheritdoc />
		protected override void OnValidate()
		{
			base.OnValidate();
			m_Data.chainWeight = Mathf.Clamp01(m_Data.chainWeight);
		}

		/// <summary>
		/// Calibrate the original spine distances for natural pose restoration
		/// </summary>
		public void CalibrateSpine()
		{
			if (m_Data.spineJoints != null && m_Data.spineJoints.Length > 0)
			{
				// Force recalibration of distances
				((IAnimationJobData)m_Data).SetDefaultValues();
			}
		}
	}

	[Unity.Burst.BurstCompile]
	public struct BasisSpineIKConstraintJob : IWeightedAnimationJob
	{
		public ReadWriteTransformHandle hips;
		public ReadWriteTransformHandle[] spineJoints;
		public ReadWriteTransformHandle head;

		public Vector3Property headTargetPosition;
		public Vector3Property headTargetRotation;
		public FloatProperty chainWeight;
		public FloatProperty jobWeight { get; set; }

		// Distance-based optimization variables
		public NativeArray<Vector3> originalDistances;
		public NativeArray<Vector3> currentDistances;
		public bool maintainSpineLength;

		public void ProcessRootMotion(AnimationStream stream) { }

		public void ProcessAnimation(AnimationStream stream)
		{
			float w = jobWeight.Get(stream);
			if (w > 0f && spineJoints.Length > 0)
			{
				Vector3 headTargetPos = headTargetPosition.Get(stream);
				Quaternion headTargetRot = Quaternion.Euler(headTargetRotation.Get(stream));
				float weight = chainWeight.Get(stream);

				// Get curvature array from transform handles
				Vector3[] curvature = new Vector3[spineJoints.Length];
				for (int i = 0; i < spineJoints.Length; i++)
				{
					curvature[i] = spineJoints[i].GetLocalPosition(stream);
				}

				// Apply PDO-IK style distance-based optimization
				SolveSpineIKWithDistanceOptimization(stream, headTargetPos, headTargetRot, curvature, w * weight);
			}
			else
			{
				// Pass through when weight is zero
				BasisAnimationRuntimeUtils.PassThrough(stream, hips);
				for (int i = 0; i < spineJoints.Length; i++)
				{
					BasisAnimationRuntimeUtils.PassThrough(stream, spineJoints[i]);
				}
				BasisAnimationRuntimeUtils.PassThrough(stream, head);
			}
		}

		private void SolveSpineIKWithDistanceOptimization(AnimationStream stream, Vector3 headTargetPos, Quaternion headTargetRot, Vector3[] curvature, float weight)
		{
			if (spineJoints.Length == 0) return;

			// Get current positions
			Vector3 hipsPos = hips.GetPosition(stream);
			Vector3 headPos = head.GetPosition(stream);

			// Calculate total spine length from original distances
			float totalLength = 0f;
			for (int i = 1; i < originalDistances.Length; i++)
			{
				totalLength += originalDistances[i].magnitude;
			}

			// Calculate direction from hips to head target
			Vector3 spineDirection = (headTargetPos - hipsPos).normalized;
			float targetDistance = Vector3.Distance(hipsPos, headTargetPos);

			// Constrain target distance if maintaining spine length
			if (maintainSpineLength && targetDistance > totalLength)
			{
				targetDistance = totalLength;
				headTargetPos = hipsPos + spineDirection * targetDistance;
			}

			// Distribute spine joints along the path using distance optimization
			for (int i = 0; i < spineJoints.Length; i++)
			{
				float t = (float)(i + 1) / (spineJoints.Length + 1);

				// Apply curvature influence (use per-joint curvature if available)
				Vector3 jointCurvature = Vector3.zero;
				if (curvature != null && i < curvature.Length)
				{
					jointCurvature = curvature[i];
				}
				Vector3 curvatureOffset = ApplyCurvature(t, jointCurvature, spineDirection);

				// Calculate target position for this joint
				Vector3 jointTargetPos = Vector3.Lerp(hipsPos, headTargetPos, t) + curvatureOffset;

				// Get current joint position and apply weighted movement
				Vector3 currentPos = spineJoints[i].GetPosition(stream);
				Vector3 newPos = Vector3.Lerp(currentPos, jointTargetPos, weight);

				// Maintain original distances if required
				if (maintainSpineLength && i > 0)
				{
					Vector3 prevPos = (i == 0) ? hipsPos : spineJoints[i - 1].GetPosition(stream);
					float originalDist = originalDistances[i + 1].magnitude;
					Vector3 constrainedPos = ConstrainDistance(prevPos, newPos, originalDist);
					newPos = Vector3.Lerp(newPos, constrainedPos, 0.5f);
				}

				spineJoints[i].SetPosition(stream, newPos);

				// Apply rotation influence
				if (i == spineJoints.Length - 1) // Last spine joint influences head rotation
				{
					Quaternion currentRot = spineJoints[i].GetRotation(stream);
					Vector3 forwardDir = (headTargetPos - newPos).normalized;
					Quaternion lookRot = Quaternion.LookRotation(forwardDir, Vector3.up);
					Quaternion blendedRot = Quaternion.Lerp(currentRot, lookRot * headTargetRot, weight * 0.5f);
					spineJoints[i].SetRotation(stream, blendedRot);
				}
			}

			// Update head position and rotation
			Vector3 finalJointPos = spineJoints[spineJoints.Length - 1].GetPosition(stream);
			Vector3 headOffset = head.GetPosition(stream) - finalJointPos;
			if (maintainSpineLength)
			{
				headOffset = headOffset.normalized * originalDistances[originalDistances.Length - 1].magnitude;
			}

			Vector3 newHeadPos = finalJointPos + headOffset;
			head.SetPosition(stream, Vector3.Lerp(head.GetPosition(stream), newHeadPos, weight));

			Quaternion newHeadRot = Quaternion.Lerp(head.GetRotation(stream), headTargetRot, weight);
			head.SetRotation(stream, newHeadRot);
		}

		private Vector3 ApplyCurvature(float t, Vector3 jointCurvature, Vector3 spineDirection)
		{
			// Apply sine wave curvature based on the PDO-IK approach
			float curvatureInfluence = Mathf.Sin(t * Mathf.PI);
			Vector3 perpendicular = Vector3.Cross(spineDirection, Vector3.up).normalized;
			if (perpendicular.magnitude < 0.1f)
			{
				perpendicular = Vector3.Cross(spineDirection, Vector3.forward).normalized;
			}

			return perpendicular * jointCurvature.x * curvatureInfluence +
				   Vector3.up * jointCurvature.y * curvatureInfluence +
				   Vector3.Cross(perpendicular, Vector3.up).normalized * jointCurvature.z * curvatureInfluence;
		}

		private Vector3 ConstrainDistance(Vector3 fromPos, Vector3 toPos, float targetDistance)
		{
			Vector3 direction = (toPos - fromPos).normalized;
			return fromPos + direction * targetDistance;
		}
	}

	public interface BasisISpineIKConstraintData
	{
		Transform hips { get; }
		Transform[] spineJoints { get; }
		Transform head { get; }

		Vector3 headTargetPosition { get; }
		Vector3 headTargetRotation { get; }
		float chainWeight { get; }

		Vector3[] originalDistances { get; }

		string chainWeightFloatProperty { get; }
		string headTargetPositionVector3Property { get; }
		string headTargetRotationVector3Property { get; }
	}

	public class BasisSpineIKConstraintJobBinder<T> : AnimationJobBinder<BasisSpineIKConstraintJob, T>
		where T : struct, IAnimationJobData, BasisISpineIKConstraintData
	{
		public override BasisSpineIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			var spineHandles = new ReadWriteTransformHandle[data.spineJoints.Length];
			for (int i = 0; i < data.spineJoints.Length; i++)
			{
				spineHandles[i] = ReadWriteTransformHandle.Bind(animator, data.spineJoints[i]);
			}

			// Create native arrays for distance optimization
			var originalDistArray = new NativeArray<Vector3>(data.originalDistances.Length, Allocator.Persistent);
			var currentDistArray = new NativeArray<Vector3>(data.originalDistances.Length, Allocator.Persistent);

			for (int i = 0; i < data.originalDistances.Length; i++)
			{
				originalDistArray[i] = data.originalDistances[i];
			}

			BasisSpineIKConstraintJob job = new BasisSpineIKConstraintJob
			{
				hips = ReadWriteTransformHandle.Bind(animator, data.hips),
				spineJoints = spineHandles,
				head = ReadWriteTransformHandle.Bind(animator, data.head),

				headTargetPosition = Vector3Property.Bind(animator, component, data.headTargetPositionVector3Property),
				headTargetRotation = Vector3Property.Bind(animator, component, data.headTargetRotationVector3Property),
				chainWeight = FloatProperty.Bind(animator, component, data.chainWeightFloatProperty),

				originalDistances = originalDistArray,
				currentDistances = currentDistArray,
				maintainSpineLength = true
			};

			return job;
		}

		public override void Destroy(BasisSpineIKConstraintJob job)
		{
			if (job.originalDistances.IsCreated)
				job.originalDistances.Dispose();
			if (job.currentDistances.IsCreated)
				job.currentDistances.Dispose();
		}
	}
}
