using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Animations.Rigging
{
	public class BasisPdoIKConstraintJobBinder<T> : AnimationJobBinder<BasisPdoIKConstraintJob, T> where T : struct, IAnimationJobData, BasisIPdoIKConstraintData
	{
		public override BasisPdoIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			int spineCount = data.spineBones.Length;

			// Create transform handles
			var spineHandles = new NativeArray<ReadWriteTransformHandle>(spineCount, Allocator.Persistent);
			for (int i = 0; i < spineCount; i++)
			{
				spineHandles[i] = ReadWriteTransformHandle.Bind(animator, data.spineBones[i]);
			}

			// Calculate DH parameters from transforms
			var dhLinkLengths = new NativeArray<float>(spineCount - 1, Allocator.Persistent);
			var dhLinkTwists = new NativeArray<float>(spineCount - 1, Allocator.Persistent);
			var dhLinkOffsets = new NativeArray<float>(spineCount - 1, Allocator.Persistent);
			var jointLimits = new NativeArray<float2>(spineCount - 1, Allocator.Persistent);

			for (int i = 0; i < spineCount - 1; i++)
			{
				dhLinkLengths[i] = Vector3.Distance(data.spineBones[i].position, data.spineBones[i + 1].position);
				dhLinkTwists[i] = 0f; // Can be configured based on anatomy
				dhLinkOffsets[i] = 0f;
				jointLimits[i] = new float2(-math.PI * 0.25f, math.PI * 0.25f); // ±45 degrees default
			}

			// Create working arrays
			var spinePositions = new NativeArray<float3>(spineCount, Allocator.Persistent);
			var spineRotations = new NativeArray<quaternion>(spineCount, Allocator.Persistent);
			var omegaSlackVariables = new NativeArray<float>(spineCount - 1, Allocator.Persistent);
			var lDistanceParameters = new NativeArray<float>(spineCount - 1, Allocator.Persistent);

			return new BasisPdoIKConstraintJob
			{
				spineHandles = spineHandles,
				headPosition = Vector3Property.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.headPosition))),
				headRotation = Vector3Property.Bind(animator, component, ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(data.headRotation))),

				dhLinkLengths = dhLinkLengths,
				dhLinkTwists = dhLinkTwists,
				dhLinkOffsets = dhLinkOffsets,
				jointLimits = jointLimits,

				convergenceThreshold = data.convergenceThreshold,
				maxIterations = data.maxIterations,
				damping = data.damping,

				spinePositions = spinePositions,
				spineRotations = spineRotations,
				omegaSlackVariables = omegaSlackVariables,
				lDistanceParameters = lDistanceParameters
			};
		}

		public override void Destroy(BasisPdoIKConstraintJob job)
		{
			if (job.spineHandles.IsCreated) job.spineHandles.Dispose();
			if (job.dhLinkLengths.IsCreated) job.dhLinkLengths.Dispose();
			if (job.dhLinkTwists.IsCreated) job.dhLinkTwists.Dispose();
			if (job.dhLinkOffsets.IsCreated) job.dhLinkOffsets.Dispose();
			if (job.jointLimits.IsCreated) job.jointLimits.Dispose();
			if (job.spinePositions.IsCreated) job.spinePositions.Dispose();
			if (job.spineRotations.IsCreated) job.spineRotations.Dispose();
			if (job.omegaSlackVariables.IsCreated) job.omegaSlackVariables.Dispose();
			if (job.lDistanceParameters.IsCreated) job.lDistanceParameters.Dispose();
		}
	}
}
