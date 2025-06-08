using System;

namespace UnityEngine.Animations.Rigging
{
    [Serializable]
    public struct BasisFullSpineIKConstraintData : IAnimationJobData, BasisIFullSpineIKConstraintData
    {
        public Transform avatarChest;
        public Transform avatarSpine;
        public Transform avatarHips;
		
		[SyncSceneToStream] public Vector3 chestPosition;
		[SyncSceneToStream] public Vector3 chestRotation;
		[SyncSceneToStream] public Vector3 spinePosition;
		[SyncSceneToStream] public Vector3 spineRotation;
		[SyncSceneToStream] public Vector3 hipsPosition;
		[SyncSceneToStream] public Vector3 hipsRotation;

		public BasisHeadChestIKConstraintData headUpperData { get; set; }

		Transform BasisIFullSpineIKConstraintData.chest { get => avatarChest;  }
		Transform BasisIFullSpineIKConstraintData.spine { get => avatarSpine; }
		Transform BasisIFullSpineIKConstraintData.hips { get => avatarHips; }

		Vector3 BasisIFullSpineIKConstraintData.chestPosition { get => chestPosition; }
		Vector3 BasisIFullSpineIKConstraintData.chestRotation { get => chestRotation; }
		Vector3 BasisIFullSpineIKConstraintData.spinePosition { get => spinePosition; }
		Vector3 BasisIFullSpineIKConstraintData.spineRotation { get => spineRotation; }
		Vector3 BasisIFullSpineIKConstraintData.hipsPosition { get => hipsPosition; }
		Vector3 BasisIFullSpineIKConstraintData.hipsRotation { get => hipsRotation; }

		public string chestPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(chestPosition));
		public string chestRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(chestRotation));
		public string spinePositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(spinePosition));
		public string spineRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(spineRotation));
		public string hipsPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(hipsPosition));
		public string hipsRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(hipsRotation));

		bool IAnimationJobData.IsValid()
		{
			return headUpperData.head != null && headUpperData.neck != null && headUpperData.chest != null && avatarChest != null && avatarSpine != null && avatarHips != null
				&& headUpperData.head.IsChildOf(headUpperData.neck) && headUpperData.neck.IsChildOf(headUpperData.chest) && headUpperData.chest.IsChildOf(avatarChest) &&
				avatarChest.IsChildOf(avatarSpine) && avatarSpine.IsChildOf(avatarHips);
		}

		void IAnimationJobData.SetDefaultValues() { }
	}
}
