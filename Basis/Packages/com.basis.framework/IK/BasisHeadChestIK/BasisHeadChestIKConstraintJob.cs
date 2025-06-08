namespace UnityEngine.Animations.Rigging
{
	[Unity.Burst.BurstCompile]
	public struct BasisHeadChestIKConstraintJob : IWeightedAnimationJob
	{
		public ReadWriteTransformHandle head;
		public ReadWriteTransformHandle neck;
		public ReadWriteTransformHandle chest;

		public Vector3Property headPosition;
		public Vector3Property headRotation;

		public Vector3Property neckPosition;
		public Vector3Property neckRotation;

		public Vector3Property chestPosition;
		public Vector3Property chestRotation;

		public FloatProperty jobWeight { get; set; }

		public void ProcessAnimation(AnimationStream stream)
		{
			float weight = jobWeight.Get(stream);
			if(weight > 0f)
			{
				AffineTransform head =
					new AffineTransform(headPosition.Get(stream), Quaternion.Euler(headRotation.Get(stream)));
				AffineTransform neck =
					new AffineTransform(neckPosition.Get(stream), Quaternion.Euler(neckRotation.Get(stream)));
				AffineTransform chest =
					new AffineTransform(chestPosition.Get(stream), Quaternion.Euler(chestRotation.Get(stream)));

				//TODO: BasisAnimationRuntimeUtils.SolveHeadChestIK();
				return;
			}

			BasisAnimationRuntimeUtils.PassThrough(stream, head);
			BasisAnimationRuntimeUtils.PassThrough(stream, neck);
			BasisAnimationRuntimeUtils.PassThrough(stream, chest);
		}

		public void ProcessRootMotion(AnimationStream stream) { }
	}
}
