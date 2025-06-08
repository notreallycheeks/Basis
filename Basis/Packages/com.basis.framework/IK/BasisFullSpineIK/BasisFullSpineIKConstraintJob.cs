using System.IO;

namespace UnityEngine.Animations.Rigging
{
    [Unity.Burst.BurstCompile]
    public struct BasisFullSpineIKConstraintJob : IWeightedAnimationJob
    {
        public ReadWriteTransformHandle avatarHead;
        public ReadWriteTransformHandle avatarNeck;
        public ReadWriteTransformHandle avatarUpperChest;
        public ReadWriteTransformHandle avatarChest;
        public ReadWriteTransformHandle avatarSpine;
        public ReadWriteTransformHandle avatarHips;

		public Vector3Property headPosition;
		public Vector3Property headRotation;

		public Vector3Property neckPosition;
		public Vector3Property neckRotation;

		public Vector3Property upperChestPosition;
		public Vector3Property upperChestRotation;

		public Vector3Property chestPosition;
		public Vector3Property chestRotation;

		public Vector3Property spinePosition;
		public Vector3Property spineRotation;

		public Vector3Property hipsPosition;
		public Vector3Property hipsRotation;

		public BasisFullSpineIKData data;

		public FloatProperty jobWeight { get; set; }

		private void PopulateData(AnimationStream stream)
		{
			AffineTransform headTarget =
				new AffineTransform(headPosition.Get(stream), Quaternion.Euler(headRotation.Get(stream)));
			AffineTransform neckTarget =
				new AffineTransform(neckPosition.Get(stream), Quaternion.Euler(neckRotation.Get(stream)));
			AffineTransform upperChestTarget =
				new AffineTransform(upperChestPosition.Get(stream), Quaternion.Euler(upperChestRotation.Get(stream)));
			AffineTransform chestTarget =
				new AffineTransform(chestPosition.Get(stream), Quaternion.Euler(chestRotation.Get(stream)));
			AffineTransform spineTarget =
				new AffineTransform(spinePosition.Get(stream), Quaternion.Euler(spineRotation.Get(stream)));
			AffineTransform hipsTarget =
				new AffineTransform(hipsPosition.Get(stream), Quaternion.Euler(hipsRotation.Get(stream)));

			data.avatarHead = avatarHead;
			data.avatarNeck = avatarNeck;
			data.avatarUpperChest = avatarUpperChest;
			data.avatarChest = avatarChest;
			data.avatarSpine = avatarSpine;
			data.avatarHips = avatarHips;

			data.headTarget = headTarget;
			data.neckTarget = neckTarget;
			data.upperChestTarget = upperChestTarget;
			data.chestTarget = chestTarget;
			data.spineTarget = spineTarget;
			data.hipsTarget = hipsTarget;
		}

        public void ProcessAnimation(AnimationStream stream)
        {
            float weight = jobWeight.Get(stream);
            if(weight > 0f)
            {
				PopulateData(stream);
				BasisFullSpineIKSolver.Solve(data);
				return;
            }

			BasisAnimationRuntimeUtils.PassThrough(stream, avatarHead);
			BasisAnimationRuntimeUtils.PassThrough(stream, avatarNeck);
			BasisAnimationRuntimeUtils.PassThrough(stream, avatarUpperChest);
			BasisAnimationRuntimeUtils.PassThrough(stream, avatarChest);
			BasisAnimationRuntimeUtils.PassThrough(stream, avatarSpine);
			BasisAnimationRuntimeUtils.PassThrough(stream, avatarHips);
		}

        public void ProcessRootMotion(AnimationStream stream) { }
    }
}
