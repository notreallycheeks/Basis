namespace UnityEngine.Animations.Rigging
{
	/// <summary>
	/// The TwoBoneIK constraint job binder.
	/// </summary>
	/// <typeparam name="T">The constraint data type</typeparam>
	public class BasisTwoBoneIKConstraintJobBinder<T> : AnimationJobBinder<BasisTwoBoneIKConstraintJob, T> where T : struct, IAnimationJobData, BasisITwoBoneIKConstraintData
	{
		/// <summary>
		/// Creates the animation job.
		/// </summary>
		/// <param name="animator">The animated hierarchy Animator component.</param>
		/// <param name="data">The constraint data.</param>
		/// <param name="component">The constraint component.</param>
		/// <returns>Returns a new job interface.</returns>
		public override BasisTwoBoneIKConstraintJob Create(Animator animator, ref T data, Component component)
		{
			BasisTwoBoneIKConstraintJob job = new BasisTwoBoneIKConstraintJob
			{
				root = ReadWriteTransformHandle.Bind(animator, data.root),
				mid = ReadWriteTransformHandle.Bind(animator, data.mid),
				tip = ReadWriteTransformHandle.Bind(animator, data.tip),
				targetPosition = Vector3Property.Bind(animator, component, data.TargetpositionVector3Property),
				targetRotation = Vector3Property.Bind(animator, component, data.TargetrotationVector3Property),

				hintPosition = Vector3Property.Bind(animator, component, data.HintpositionVector3Property),
				hintRotation = Vector3Property.Bind(animator, component, data.HintrotationVector3Property),

				targetOffset = AffineTransform.identity,
			};

			job.targetOffset.translation = data.CalibratedOffset;
			job.targetOffset.rotation = Quaternion.Euler(data.CalibratedRotation);
			job.hintWeight = BoolProperty.Bind(animator, component, data.hintWeightFloatProperty);
			job.BendNormal = Vector3Property.Bind(animator, component, data.HintDirectionProperty);

			return job;
		}

		/// <summary>
		/// Destroys the animation job.
		/// </summary>
		/// <param name="job">The animation job to destroy.</param>
		public override void Destroy(BasisTwoBoneIKConstraintJob job) { }
	}
}
