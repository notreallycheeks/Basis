namespace UnityEngine.Animations.Rigging
{
    public interface BasisIFullSpineIKConstraintData
    {
        public BasisHeadChestIKConstraintData headUpperData { get; }

        public Transform chest { get; }
        public Transform spine { get; }
        public Transform hips { get; }

        public Vector3 chestPosition { get; }
        public Vector3 chestRotation { get; }
        public Vector3 spinePosition { get; }
        public Vector3 spineRotation { get; }
        public Vector3 hipsPosition { get; }
        public Vector3 hipsRotation { get; }

        string chestPositionVector3Property { get; }
        string chestRotationVector3Property { get; }
		string spinePositionVector3Property { get; }
		string spineRotationVector3Property { get; }
		string hipsPositionVector3Property { get; }
		string hipsRotationVector3Property { get; }
	}
}
