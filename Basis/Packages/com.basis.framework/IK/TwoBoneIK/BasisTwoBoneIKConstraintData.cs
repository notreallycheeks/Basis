namespace UnityEngine.Animations.Rigging
{
	public struct BasisTwoBoneIKConstraintData : IAnimationJobData, BasisITwoBoneIKConstraintData
	{
		[SerializeField] Transform m_Root;
		[SerializeField] Transform m_Mid;
		[SerializeField] Transform m_Tip;

		[SyncSceneToStream, SerializeField]
		public Vector3 TargetPosition;
		[SyncSceneToStream, SerializeField]
		public Vector3 TargetRotation;
		[SyncSceneToStream, SerializeField]
		public Vector3 HintPosition;
		[SyncSceneToStream, SerializeField]
		public Vector3 HintRotation;

		Vector3 BasisITwoBoneIKConstraintData.targetPosition { get => TargetPosition; }

		Vector3 BasisITwoBoneIKConstraintData.targetRotation { get => TargetRotation; }

		Vector3 BasisITwoBoneIKConstraintData.hintPosition { get => HintPosition; }
		Vector3 BasisITwoBoneIKConstraintData.HintRotation { get => HintRotation; }
		[SyncSceneToStream, SerializeField]
		bool m_HintWeight;
		/// <inheritdoc />
		public Transform root { get => m_Root; set => m_Root = value; }
		/// <inheritdoc />
		public Transform mid { get => m_Mid; set => m_Mid = value; }
		/// <inheritdoc />
		public Transform tip { get => m_Tip; set => m_Tip = value; }
		/// <inheritdoc />
		/// <summary>The weight for which hint transform has an effect on IK calculations. This is a value in between 0 and 1.</summary>
		public bool hintWeight { get => m_HintWeight; set => m_HintWeight = value; }
		/// <inheritdoc />
		string BasisITwoBoneIKConstraintData.hintWeightFloatProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_HintWeight));

		string BasisITwoBoneIKConstraintData.TargetpositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(TargetPosition));

		string BasisITwoBoneIKConstraintData.TargetrotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(TargetRotation));

		string BasisITwoBoneIKConstraintData.HintpositionVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(HintPosition));

		string BasisITwoBoneIKConstraintData.HintrotationVector3Property => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(HintRotation));

		string BasisITwoBoneIKConstraintData.HintDirectionProperty => ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(m_HintDirection));

		[SyncSceneToStream, SerializeField]
		public Vector3 M_CalibratedOffset;
		[SyncSceneToStream, SerializeField]
		public Vector3 M_CalibratedRotation;

		public Vector3 CalibratedOffset
		{
			get
			{
				return M_CalibratedOffset;
			}
		}

		public Vector3 CalibratedRotation
		{
			get
			{
				return M_CalibratedRotation;
			}
		}
		[SyncSceneToStream, SerializeField]
		public Vector3 m_HintDirection;
		Vector3 BasisITwoBoneIKConstraintData.HintDirection
		{
			get
			{
				return m_HintDirection;
			}
		}

		/// <inheritdoc />
		bool IAnimationJobData.IsValid() => (m_Tip != null && m_Mid != null && m_Root != null && m_Tip.IsChildOf(m_Mid) && m_Mid.IsChildOf(m_Root));

		/// <inheritdoc />
		void IAnimationJobData.SetDefaultValues()
		{
			m_Root = null;
			m_Mid = null;
			m_Tip = null;
			m_HintWeight = true;
		}
	}
}
