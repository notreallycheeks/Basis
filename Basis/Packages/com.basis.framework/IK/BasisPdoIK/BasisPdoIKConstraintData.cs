using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct BasisPdoIKConstraintData : IAnimationJobData, BasisIPdoIKConstraintData
	{
		[SerializeField] Transform avatarHips;
		[SerializeField] Transform avatarHead;
		[SerializeField] Transform[] avatarSpineBones;

		[SyncSceneToStream] Vector3 avatarHeadPosition;
		[SyncSceneToStream] Vector3 avatarHeadRotation;

		[SerializeField] float m_ConvergenceThreshold;
		[SerializeField] int m_MaxIterations;
		[SerializeField] float m_Damping;

		public Transform hips { get => avatarHips; set => avatarHips = value; }
		public Transform head { get => avatarHead; set => avatarHead = value; }
		public Transform[] spineBones { get => avatarSpineBones; set => avatarSpineBones = value; }

		public Vector3 headPosition { get => avatarHeadPosition; set => avatarHeadPosition = value; }
		public Vector3 headRotation { get => avatarHeadRotation; set => avatarHeadRotation = value; }

		public string headPositionVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headPosition));
		public string headRotationVector3Property
			=> ConstraintsUtils.ConstructConstraintDataPropertyName(nameof(headRotation));

		public float convergenceThreshold { get => m_ConvergenceThreshold; set => m_ConvergenceThreshold = value; }
		public int maxIterations { get => m_MaxIterations; set => m_MaxIterations = value; }
		public float damping { get => m_Damping; set => m_Damping = value; }

		bool IAnimationJobData.IsValid()
		{
			return avatarHips != null && avatarHead != null &&
				   avatarSpineBones != null && avatarSpineBones.Length > 0;
		}

		void IAnimationJobData.SetDefaultValues() { }
	}
}
