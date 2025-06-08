namespace UnityEngine.Animations.Rigging
{
	/// <summary>
	/// The TwoBoneIK constraint job.
	/// </summary>
	[Unity.Burst.BurstCompile]
	public struct BasisTwoBoneIKConstraintJob : IWeightedAnimationJob
	{
		/// <summary>The transform handle for the root transform.</summary>
		public ReadWriteTransformHandle root;
		/// <summary>The transform handle for the mid transform.</summary>
		public ReadWriteTransformHandle mid;
		/// <summary>The transform handle for the tip transform.</summary>
		public ReadWriteTransformHandle tip;

		/// <summary>The transform handle for the hint transform.</summary>
		public Vector3Property hintPosition;
		/// <summary>The transform handle for the target transform.</summary>
		public Vector3Property targetPosition;

		/// <summary>The transform handle for the hint transform.</summary>
		public Vector3Property hintRotation;
		/// <summary>The transform handle for the target transform.</summary>
		public Vector3Property targetRotation;

		/// <summary>The offset applied to the target transform if maintainTargetPositionOffset or maintainTargetRotationOffset is enabled.</summary>
		public AffineTransform targetOffset;
		/// <summary>The weight for which hint transform has an effect on IK calculations. This is a value in between 0 and 1.</summary>
		public BoolProperty hintWeight;

		/// <summary>The main weight given to the constraint. This is a value in between 0 and 1.</summary>
		public FloatProperty jobWeight { get; set; }

		/// <summary>The transform handle for the hint transform.</summary>
		public Vector3Property BendNormal;
		/// <summary>
		/// Defines what to do when processing the root motion.
		/// </summary>
		/// <param name="stream">The animation stream to work on.</param>
		public void ProcessRootMotion(AnimationStream stream) { }

		/// <summary>
		/// Defines what to do when processing the animation.
		/// </summary>
		/// <param name="stream">The animation stream to work on.</param>
		public void ProcessAnimation(AnimationStream stream)
		{
			float w = jobWeight.Get(stream);
			if (w > 0f)
			{
				AffineTransform target = new AffineTransform(targetPosition.Get(stream), Quaternion.Euler(targetRotation.Get(stream)));
				AffineTransform hint = new AffineTransform(hintPosition.Get(stream), Quaternion.Euler(hintRotation.Get(stream)));
				Vector3 BendNormalOutput = BendNormal.Get(stream);

				BasisAnimationRuntimeUtils.SolveTwoBoneIKLegsAndTorso(stream, root, mid, tip, target, hint, hintWeight.Get(stream), targetOffset, BendNormalOutput);
			}
			else
			{
				BasisAnimationRuntimeUtils.PassThrough(stream, root);
				BasisAnimationRuntimeUtils.PassThrough(stream, mid);
				BasisAnimationRuntimeUtils.PassThrough(stream, tip);
			}
		}
	}
}
