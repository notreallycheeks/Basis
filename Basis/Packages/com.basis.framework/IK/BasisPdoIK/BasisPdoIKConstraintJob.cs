using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct BasisPdoIKConstraintJob : IWeightedAnimationJob
	{
		public NativeArray<ReadWriteTransformHandle> spineHandles;

		// Target data
		public Vector3Property headPosition;
		public Vector3Property headRotation;

		// Job weight
		public FloatProperty jobWeight { get; set; }

		// IK job data
		public NativeArray<float> dhLinkLengths;
		public NativeArray<float> dhLinkTwists;
		public NativeArray<float> dhLinkOffsets;
		public NativeArray<float2> jointLimits;
		public float convergenceThreshold;
		public int maxIterations;
		public float damping;

		// Working arrays (allocated once, reused)
		[NativeDisableParallelForRestriction] public NativeArray<float3> spinePositions;
		[NativeDisableParallelForRestriction] public NativeArray<quaternion> spineRotations;
		[NativeDisableParallelForRestriction] public NativeArray<float> omegaSlackVariables;
		[NativeDisableParallelForRestriction] public NativeArray<float> lDistanceParameters;

		public void ProcessAnimation(AnimationStream stream)
		{
			float weight = jobWeight.Get(stream);
			if (weight <= 0f || spineHandles.Length < 2) return;

			// Get current spine positions and rotations
			for (int i = 0; i < spineHandles.Length; i++)
			{
				spinePositions[i] = spineHandles[i].GetPosition(stream);
				spineRotations[i] = spineHandles[i].GetRotation(stream);
			}

			// Get target head pose
			float3 targetHeadPos = headPosition.Get(stream);
			quaternion targetHeadRot = quaternion.Euler(math.radians(headRotation.Get(stream)));

			// Solve spine IK using PDO-IK algorithm
			SolvePdoIK(targetHeadPos, targetHeadRot);

			// Apply results with weight blending
			for (int i = 0; i < spineHandles.Length; i++)
			{
				Vector3 currentPos = spineHandles[i].GetPosition(stream);
				Quaternion currentRot = spineHandles[i].GetRotation(stream);

				Vector3 targetPos = math.lerp(currentPos, spinePositions[i], weight);
				Quaternion targetRotQuat = math.slerp(currentRot, spineRotations[i], weight);

				spineHandles[i].SetPosition(stream, targetPos);
				spineHandles[i].SetRotation(stream, targetRotQuat);
			}
		}

		private void SolvePdoIK(float3 targetHeadPosition, quaternion targetHeadRotation)
		{
			int spineJointCount = spinePositions.Length;
			if (spineJointCount < 2) return;

			// Initialize slack variables to neutral position
			for (int i = 0; i < omegaSlackVariables.Length; i++)
			{
				omegaSlackVariables[i] = 0f;
			}

			float currentError = math.distance(targetHeadPosition, spinePositions[spineJointCount - 1]);
			int iteration = 0;

			// PDO-IK iterative solver
			while (currentError > convergenceThreshold && iteration < maxIterations)
			{
				// Forward rollout - compute current spine configuration
				ForwardRollout();

				// Compute Jacobian and update slack variables
				ComputeJacobianUpdate(targetHeadPosition);

				// Check convergence
				currentError = math.distance(targetHeadPosition, spinePositions[spineJointCount - 1]);
				iteration++;
			}

			Debug.Log("BasisPdoIKConstraintJob: SolvePdoIK Completed.");
		}

		private void ForwardRollout()
		{
			// Start from hip (already set in spinePositions[0] and spineRotations[0])
			float4x4 currentTransform = float4x4.TRS(spinePositions[0], spineRotations[0], new float3(1f));

			// Propagate along spine chain using distance-based formulation
			for (int i = 0; i < omegaSlackVariables.Length; i++)
			{
				// Convert slack variable to L parameter using squashing function
				float L_i = SquashingFunction(omegaSlackVariables[i], jointLimits[i]);
				lDistanceParameters[i] = L_i;

				// Compute joint transformation using distance-based DH formulation
				float4x4 jointTransform = ComputeJointTransform(L_i, dhLinkLengths[i], dhLinkTwists[i], dhLinkOffsets[i]);
				currentTransform = math.mul(currentTransform, jointTransform);

				// Extract position and rotation for next spine segment
				int outputIndex = i + 1;
				spinePositions[outputIndex] = currentTransform.c3.xyz;
				spineRotations[outputIndex] = new quaternion(currentTransform);
			}
		}

		private void ComputeJacobianUpdate(float3 targetHeadPosition)
		{
			int lastIndex = spinePositions.Length - 1;
			float3 endEffectorPos = spinePositions[lastIndex];
			float3 positionError = targetHeadPosition - endEffectorPos;

			// Simplified Jacobian computation for real-time performance
			for (int i = 0; i < omegaSlackVariables.Length; i++)
			{
				float3 jointPos = spinePositions[i];
				float3 toEndEffector = endEffectorPos - jointPos;

				// Spine primarily rotates around local axes - use cross product for rotation axis
				float3 axis = math.normalize(math.cross(math.up(), toEndEffector));
				if (math.lengthsq(axis) < 0.001f) // Handle near-parallel case
				{
					axis = math.right(); // Fallback axis
				}

				float3 jacobianColumn = math.cross(axis, toEndEffector);

				// Compute update step
				float deltaOmega = math.dot(math.normalize(jacobianColumn + new float3(0.001f)), positionError) * damping;

				// Apply update (joint limits handled by squashing function)
				omegaSlackVariables[i] += deltaOmega;
			}
		}

		private float SquashingFunction(float omega, float2 limits)
		{
			// Convert angle limits to L parameter range
			float minL = AngleToL(limits.x);
			float maxL = AngleToL(limits.y);

			// Sigmoid squashing function to enforce bounds smoothly
			float sigmoid = 1.0f / (1.0f + math.exp(-omega));
			return minL + (maxL - minL) * sigmoid;
		}

		private float AngleToL(float angle)
		{
			// Convert angle to L parameter: L = 2 - 2*cos(angle), clamped to [0, 2]
			return math.clamp(2.0f - 2.0f * math.cos(angle), 0f, 2f);
		}

		private float4x4 ComputeJointTransform(float L_i, float linkLength, float linkTwist, float linkOffset)
		{
			// Distance-based DH transformation (Equation 4.3 from PDO-IK paper)
			float cosTheta = 1.0f - L_i;
			float sinTheta = math.sqrt(math.max(0f, 2.0f * L_i - L_i * L_i));

			float ca = math.cos(linkTwist);
			float sa = math.sin(linkTwist);

			return new float4x4(
				new float4(cosTheta, -sinTheta, 0, linkLength),
				new float4(sinTheta * ca, cosTheta * ca, -sa, linkOffset * sa),
				new float4(sinTheta * sa, cosTheta * sa, ca, linkOffset * ca),
				new float4(0, 0, 0, 1)
			);
		}

		public void ProcessRootMotion(AnimationStream stream) { }
	}
}
