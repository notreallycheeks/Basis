namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Full Spine Constraint")]
	public class BasisFullSpineIKConstraint : RigConstraint<BasisFullSpineIKConstraintJob, BasisFullSpineIKConstraintData, BasisFullSpineIKConstraintJobBinder<BasisFullSpineIKConstraintData>>
    {
		protected override void OnValidate()
		{
			base.OnValidate();
		}
	}
}
