namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Basis Spine IK Constraint")]
	public class BasisPdoIKConstraint : RigConstraint<BasisPdoIKConstraintJob, BasisPdoIKConstraintData, BasisPdoIKConstraintJobBinder<BasisPdoIKConstraintData>>
	{
		protected override void OnValidate()
		{
			base.OnValidate();
		}
	}
}

