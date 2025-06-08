namespace UnityEngine.Animations.Rigging
{
	/// <summary>
	/// This interface defines the data mapping for the TwoBoneIK constraint.
	/// </summary>
	public interface BasisITwoBoneIKConstraintData
	{
		/// <summary>The root transform of the two bones hierarchy.</summary>
		Transform root { get; }
		/// <summary>The mid transform of the two bones hierarchy.</summary>
		Transform mid { get; }
		/// <summary>The tip transform of the two bones hierarchy.</summary>
		Transform tip { get; }
		public Vector3 targetPosition { get; }
		public Vector3 targetRotation { get; }
		public Vector3 hintPosition { get; }
		public Vector3 HintRotation { get; }

		public Vector3 CalibratedOffset { get; }
		public Vector3 CalibratedRotation { get; }
		/// <summary>The path to the hint weight property in the constraint component.</summary>
		string hintWeightFloatProperty { get; }

		/// <summary>The path to the override position property in the constraint component.</summary>
		string TargetpositionVector3Property { get; }
		/// <summary>The path to the override rotation property in the constraint component.</summary>
		string TargetrotationVector3Property { get; }

		/// <summary>The path to the override position property in the constraint component.</summary>
		string HintpositionVector3Property { get; }
		/// <summary>The path to the override rotation property in the constraint component.</summary>
		string HintrotationVector3Property { get; }
		string HintDirectionProperty { get; }

		public Vector3 HintDirection { get; }

	}
}
