using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct BasisSlinkySpineIKConstraintData : IAnimationJobData, BasisISlinkySpineIKConstraintData
	{
		[SerializeField] Transform m_Root;
		[SerializeField] Transform m_Mid;
		[SerializeField] Transform m_Tip;

		[SyncSceneToStream, SerializeField] bool m_HintWeight;
		[SyncSceneToStream, SerializeField] public Vector3 m_HintDirection;

		public Transform root { get => m_Root; set => m_Root = value; }
		public Transform mid { get => m_Mid; set => m_Mid = value; }
		public Transform tip { get => m_Tip; set => m_Tip = value; }
		public bool hintWeight { get => m_HintWeight; set => m_HintWeight = value; }

		[SyncSceneToStream, SerializeField] public Vector3 rootTargetPosition;
		[SyncSceneToStream, SerializeField] public Vector3 rootTargetRotation;
		[SyncSceneToStream, SerializeField] public Vector3 midTargetPosition;
		[SyncSceneToStream, SerializeField] public Vector3 midTargetRotation;
		[SyncSceneToStream, SerializeField] public Vector3 tipTargetPosition;
		[SyncSceneToStream, SerializeField] public Vector3 tipTargetRotation;
		[SyncSceneToStream, SerializeField] public Vector3 hintPosition;
		[SyncSceneToStream, SerializeField] public Vector3 hintRotation;

		public Vector3 calibratedOffset { get; set; }
		public Vector3 calibratedRotation { get; set; }

		Vector3 BasisISlinkySpineIKConstraintData.rootTargetPosition { get => rootTargetPosition; }
		Vector3 BasisISlinkySpineIKConstraintData.rootTargetRotation { get => rootTargetRotation; }
		Vector3 BasisISlinkySpineIKConstraintData.midTargetPosition { get => midTargetPosition; }
		Vector3 BasisISlinkySpineIKConstraintData.midTargetRotation { get => midTargetRotation; }
		Vector3 BasisISlinkySpineIKConstraintData.tipTargetPosition { get => tipTargetPosition; }
		Vector3 BasisISlinkySpineIKConstraintData.tipTargetRotation { get => tipTargetRotation; }
		Vector3 BasisISlinkySpineIKConstraintData.hintPosition { get => hintPosition; }
		Vector3 BasisISlinkySpineIKConstraintData.hintRotation { get => hintRotation; }
		Vector3 BasisISlinkySpineIKConstraintData.hintDirection { get => m_HintDirection; }

		public string hintWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_HintWeight));
		public string hintDirectionProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_HintDirection));
		public string hintPositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(hintPosition));
		public string hintRotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(hintRotation));
		public string tipTargetPositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(tipTargetPosition));
		public string tipTargetRotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(tipTargetRotation));
		public string midTargetPositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(midTargetPosition));
		public string midTargetRotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(midTargetRotation));
		public string rootTargetPositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(rootTargetPosition));
		public string rootTargetRotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(rootTargetRotation));

		public bool IsValid() => (m_Tip != null && m_Mid != null && m_Root != null && m_Tip.IsChildOf(m_Mid) && m_Mid.IsChildOf(m_Root));

		public void SetDefaultValues()
		{
			m_Root = null;
			m_Mid = null;
			m_Tip = null;
			m_HintWeight = true;
		}
	}

	[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Slink Spine IK Constraint")]
	public class BasisSlinkySpineIKConstraint : RigConstraint<BasisSlinkySpineIKConstraintJob, BasisSlinkySpineIKConstraintData, BasisSlinkySpineIKConstraintJobBinder<BasisSlinkySpineIKConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
			m_Data.hintWeight = m_Data.hintWeight;
		}
	}

	[Unity.Burst.BurstCompile]
	public struct BasisSlinkySpineIKConstraintJob : IWeightedAnimationJob
	{
		public ReadWriteTransformHandle root;
		public ReadWriteTransformHandle mid;
		public ReadWriteTransformHandle tip;

		public Vector3Property hintPosition;
		public Vector3Property hintRotation;

		public Vector3Property tipTargetPosition;
		public Vector3Property tipTargetRotation;

		public Vector3Property midTargetPosition;
		public Vector3Property midTargetRotation;

		public Vector3Property rootTargetPosition;
		public Vector3Property rootTargetRotation;

		public AffineTransform targetOffset;

		public BoolProperty hintWeight;
		public Vector3Property hintTransform;

		public FloatProperty jobWeight { get; set; }

		public void ProcessRootMotion(AnimationStream stream) { }

		public void ProcessAnimation(AnimationStream stream)
		{
			float w = jobWeight.Get(stream);
			if(w > 0f)
			{
				AffineTransform tipTarget = new AffineTransform(tipTargetPosition.Get(stream), Quaternion.Euler(tipTargetRotation.Get(stream)));
				AffineTransform midTarget = new AffineTransform(midTargetPosition.Get(stream), Quaternion.Euler(midTargetRotation.Get(stream)));
				AffineTransform rootTarget = new AffineTransform(rootTargetPosition.Get(stream), Quaternion.Euler(rootTargetRotation.Get(stream)));

				AffineTransform hint = new AffineTransform(hintPosition.Get(stream), Quaternion.Euler(hintRotation.Get(stream)));
				Vector3 hintTransformOutput = hintTransform.Get(stream);

				BasisAnimationRuntimeUtils.SolveSlinkySpineIK(stream, root, mid, tip, rootTarget, midTarget, tipTarget, hint, hintWeight.Get(stream), targetOffset, hintTransformOutput);
				return;
			}

			BasisAnimationRuntimeUtils.PassThrough(stream, root);
			BasisAnimationRuntimeUtils.PassThrough(stream, mid);
			BasisAnimationRuntimeUtils.PassThrough(stream, tip);
		}

	}

	public interface BasisISlinkySpineIKConstraintData
	{
		Transform root { get; }
		Transform mid { get; }
		Transform tip { get; }

		public Vector3 rootTargetPosition { get; }
		public Vector3 rootTargetRotation { get; }
		public Vector3 midTargetPosition { get; }
		public Vector3 midTargetRotation { get; }
		public Vector3 tipTargetPosition { get; }
		public Vector3 tipTargetRotation { get; }

		public Vector3 hintPosition { get; }
		public Vector3 hintRotation { get; }
		public Vector3 hintDirection { get; }

		public Vector3 calibratedOffset { get; }
		public Vector3 calibratedRotation { get; }

		string hintWeightFloatProperty { get; }
		string hintDirectionProperty { get; }
		string hintPositionVector3Property { get; }
		string hintRotationVector3Property { get; }
		string tipTargetPositionVector3Property { get; }
		string tipTargetRotationVector3Property { get; }
		string midTargetPositionVector3Property { get; }
		string midTargetRotationVector3Property { get; }
		string rootTargetPositionVector3Property { get; }
		string rootTargetRotationVector3Property { get; }
	}

	public class BasisSlinkySpineIKConstraintJobBinder<T> : AnimationJobBinder<BasisSlinkySpineIKConstraintJob, T> where T : struct, IAnimationJobData, BasisISlinkySpineIKConstraintData
	{
		public override BasisSlinkySpineIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			BasisSlinkySpineIKConstraintJob job = new BasisSlinkySpineIKConstraintJob
			{
				root = ReadWriteTransformHandle.Bind(animator, data.root),
				mid = ReadWriteTransformHandle.Bind(animator, data.mid),
				tip = ReadWriteTransformHandle.Bind(animator, data.tip),

				tipTargetPosition = Vector3Property.Bind(animator, component, data.tipTargetPositionVector3Property),
				tipTargetRotation = Vector3Property.Bind(animator, component, data.tipTargetRotationVector3Property),
				midTargetPosition = Vector3Property.Bind(animator, component, data.midTargetPositionVector3Property),
				midTargetRotation = Vector3Property.Bind(animator, component, data.midTargetRotationVector3Property),
				rootTargetPosition = Vector3Property.Bind(animator, component, data.rootTargetPositionVector3Property),
				rootTargetRotation = Vector3Property.Bind(animator, component, data.rootTargetRotationVector3Property),

				hintPosition = Vector3Property.Bind(animator, component, data.hintPositionVector3Property),
				hintRotation = Vector3Property.Bind(animator, component, data.hintRotationVector3Property),

				targetOffset = AffineTransform.identity
			};

			job.targetOffset.translation = data.calibratedOffset;
			job.targetOffset.rotation = Quaternion.Euler(data.calibratedRotation);
			job.hintWeight = BoolProperty.Bind(animator, component, data.hintWeightFloatProperty);
			job.hintTransform = Vector3Property.Bind(animator, component, data.hintDirectionProperty);

			return job;
		}

		public override void Destroy(BasisSlinkySpineIKConstraintJob job) { }
	}
}

