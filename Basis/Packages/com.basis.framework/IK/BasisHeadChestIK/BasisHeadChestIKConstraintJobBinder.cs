
namespace UnityEngine.Animations.Rigging
{
    public class BasisHeadChestIKConstraintJobBinder<T> : AnimationJobBinder<BasisHeadChestIKConstraintJob, T> where T : struct, IAnimationJobData, BasisIHeadChestIKConstraintData
	{
		public override BasisHeadChestIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			BasisHeadChestIKConstraintJob job = new BasisHeadChestIKConstraintJob
			{
				head = ReadWriteTransformHandle.Bind(animator, data.head),
				neck = ReadWriteTransformHandle.Bind(animator, data.neck),
				chest = ReadWriteTransformHandle.Bind(animator, data.chest),

				headPosition = Vector3Property.Bind(animator, component, data.headPositionVector3Property),
				headRotation = Vector3Property.Bind(animator, component, data.headRotationVector3Property),
				neckPosition = Vector3Property.Bind(animator, component, data.neckPositionVector3Property),
				neckRotation = Vector3Property.Bind(animator, component, data.neckRotationVector3Property),
				chestPosition = Vector3Property.Bind(animator, component, data.chestPositionVector3Property),
				chestRotation = Vector3Property.Bind(animator, component, data.chestRotationVector3Property),
			};

			return job;
		}

		public override void Destroy(BasisHeadChestIKConstraintJob job) { }
	}
}
