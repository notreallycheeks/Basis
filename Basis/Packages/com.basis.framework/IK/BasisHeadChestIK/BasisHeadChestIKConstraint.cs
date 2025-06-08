namespace UnityEngine.Animations.Rigging
{
	[DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Head Chest Constraint")]
	public class BasisHeadChestIKConstraint : RigConstraint<BasisHeadChestIKConstraintJob, BasisHeadChestIKConstraintData, BasisHeadChestIKConstraintJobBinder<BasisHeadChestIKConstraintData>>
    {
        protected override void OnValidate()
		{ 
			base.OnValidate();
		}
	}
}
