namespace UnityEngine.Animations.Rigging
{
    [DisallowMultipleComponent, AddComponentMenu("Animation Rigging/Two Bone IK Constraint")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/TwoBoneIKConstraint.html")]
    public class BasisTwoBoneIKConstraint : RigConstraint<BasisTwoBoneIKConstraintJob, BasisTwoBoneIKConstraintData, BasisTwoBoneIKConstraintJobBinder<BasisTwoBoneIKConstraintData>>
    {
        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Data.hintWeight = m_Data.hintWeight;
        }
    }
}
