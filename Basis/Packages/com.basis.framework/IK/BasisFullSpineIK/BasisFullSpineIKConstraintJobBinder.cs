
namespace UnityEngine.Animations.Rigging
{
    public class BasisFullSpineIKConstraintJobBinder<T> : AnimationJobBinder<BasisFullSpineIKConstraintJob, T> where T : struct, IAnimationJobData, BasisIFullSpineIKConstraintData
    {
		public override BasisFullSpineIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			BasisFullSpineIKConstraintJob job = new BasisFullSpineIKConstraintJob
			{
				avatarHead = ReadWriteTransformHandle.Bind(animator, data.headUpperData.head),
				avatarNeck = ReadWriteTransformHandle.Bind(animator, data.headUpperData.neck),
				avatarUpperChest = ReadWriteTransformHandle.Bind(animator, data.headUpperData.chest),
				avatarChest = ReadWriteTransformHandle.Bind(animator, data.chest),
				avatarSpine = ReadWriteTransformHandle.Bind(animator, data.spine),
				avatarHips = ReadWriteTransformHandle.Bind(animator, data.hips),

				headPosition = Vector3Property.Bind(animator, component, data.headUpperData.headPositionVector3Property),
				headRotation = Vector3Property.Bind(animator, component, data.headUpperData.headRotationVector3Property),
				neckPosition = Vector3Property.Bind(animator, component, data.headUpperData.neckPositionVector3Property),
				neckRotation = Vector3Property.Bind(animator, component, data.headUpperData.neckRotationVector3Property),
				upperChestPosition = Vector3Property.Bind(animator, component, data.headUpperData.chestPositionVector3Property),
				upperChestRotation = Vector3Property.Bind(animator, component, data.headUpperData.chestRotationVector3Property),
				chestPosition = Vector3Property.Bind(animator, component, data.chestPositionVector3Property),
				chestRotation = Vector3Property.Bind(animator, component, data.chestRotationVector3Property),
				spinePosition = Vector3Property.Bind(animator, component, data.spinePositionVector3Property),
				spineRotation = Vector3Property.Bind(animator, component, data.spineRotationVector3Property),
				hipsPosition = Vector3Property.Bind(animator, component, data.hipsPositionVector3Property),
				hipsRotation = Vector3Property.Bind(animator, component, data.hipsRotationVector3Property),
			};

			return job;
		}

		public override void Destroy(BasisFullSpineIKConstraintJob job) { }
	}
}
