namespace UnityEngine.Animations.Rigging
{
    public interface BasisIHeadChestIKConstraintData
    {
        Transform head { get; }
		Transform neck { get; }
		Transform chest { get; }

        public Vector3 headPosition { get; }
        public Vector3 headRotation { get; }
        public Vector3 neckPosition { get; }
        public Vector3 neckRotation { get; }
        public Vector3 chestPosition { get; }
        public Vector3 chestRotation { get; }

        string headPositionVector3Property { get; }
		string headRotationVector3Property { get; }
		string neckPositionVector3Property { get; }
		string neckRotationVector3Property { get; }
		string chestPositionVector3Property { get; }
		string chestRotationVector3Property { get; }
	}
}
