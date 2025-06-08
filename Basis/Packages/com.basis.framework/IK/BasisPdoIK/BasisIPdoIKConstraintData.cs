namespace UnityEngine.Animations.Rigging
{
    public interface BasisIPdoIKConstraintData
    {
        public Transform head { get; }
        public Transform hips { get; }
        public Transform[] spineBones { get; }

        public Vector3 headPosition { get; }
        public Vector3 headRotation { get; }

        string headPositionVector3Property { get; }
		string headRotationVector3Property { get; }

        float convergenceThreshold { get; }
		float damping { get; }
        int maxIterations { get; }
	}
}
