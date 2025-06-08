
using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct BasisHeadChestIKConstraintData : IAnimationJobData, BasisIHeadChestIKConstraintData
	{
		public Transform head;
		public Transform neck;
		public Transform chest;

		[SyncSceneToStream] public Vector3 headPosition;
		[SyncSceneToStream] public Vector3 headRotation;
		[SyncSceneToStream] public Vector3 neckPosition;
		[SyncSceneToStream] public Vector3 neckRotation;
		[SyncSceneToStream] public Vector3 chestPosition;
		[SyncSceneToStream] public Vector3 chestRotation;

		Transform BasisIHeadChestIKConstraintData.head { get => head; }
		Transform BasisIHeadChestIKConstraintData.neck { get => neck; }
		Transform BasisIHeadChestIKConstraintData.chest { get => chest; }

		Vector3 BasisIHeadChestIKConstraintData.headPosition { get => headPosition; }
		Vector3 BasisIHeadChestIKConstraintData.headRotation { get => headRotation; }
		Vector3 BasisIHeadChestIKConstraintData.neckPosition { get => neckPosition; }
		Vector3 BasisIHeadChestIKConstraintData.neckRotation { get => neckRotation; }
		Vector3 BasisIHeadChestIKConstraintData.chestPosition { get => chestPosition; }
		Vector3 BasisIHeadChestIKConstraintData.chestRotation { get => chestRotation; }

		public string headPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headPosition));
		public string headRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headRotation));
		public string neckPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(neckPosition));
		public string neckRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(neckRotation));
		public string chestPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(chestPosition));
		public string chestRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(chestRotation));

		bool IAnimationJobData.IsValid()
		{
			return head != null && neck != null && chest != null && head.IsChildOf(neck) && neck.IsChildOf(chest);
		}

		void IAnimationJobData.SetDefaultValues()
		{
			head = null;
			neck = null;
			chest = null;
		}
	}
}
