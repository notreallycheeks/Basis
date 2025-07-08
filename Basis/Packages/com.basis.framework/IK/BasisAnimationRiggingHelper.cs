using Basis.Scripts.BasisSdk.Helpers;
using Basis.Scripts.BasisSdk.Players;
using Basis.Scripts.Drivers;
using Basis.Scripts.TransformBinders.BoneControl;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public static class BasisAnimationRiggingHelper
{
    public static void EnableTwoBoneIk(BasisTwoBoneIKConstraint Constraint, Vector3 TargetPositionOffset, Vector3 TargetRotationOffset)
    {
        Constraint.data.M_CalibratedOffset = TargetPositionOffset;
        Constraint.data.M_CalibratedRotation = TargetRotationOffset;
    }

    public static BasisApplyTranslation Damp(BasisLocalPlayer player, GameObject Parent, Transform Source, BasisBoneTrackedRole Role, float rotationWeight = 1, float positionWeight = 1)
    {
        player.LocalBoneDriver.FindBone(out BasisLocalBoneControl Target, Role);
        GameObject DTData = CreateAndSetParent(Parent.transform, $"Bone Role {Role.ToString()}");
        BasisApplyTranslation DT = BasisHelpers.GetOrAddComponent<BasisApplyTranslation>(DTData);

        DT.data.constrainedObject = Source;
        GenerateRequiredTransforms(player, Source);
        WriteUpWeights(Target, DT);
        return DT;
    }

    public static void TwistChain(BasisLocalBoneDriver driver, GameObject Parent, Transform root, Transform tip, BasisBoneTrackedRole Root, BasisBoneTrackedRole Tip, float rotationWeight = 1, float positionWeight = 1)
    {
        driver.FindBone(out BasisLocalBoneControl RootTarget, Root);
        driver.FindBone(out BasisLocalBoneControl TipTarget, Tip);
        GameObject DTData = CreateAndSetParent(Parent.transform, $"Bone Role {Root.ToString()}");
        TwistChainConstraint DT = BasisHelpers.GetOrAddComponent<TwistChainConstraint>(DTData);
        Keyframe[] Frame = new Keyframe[2];
        Frame[0] = new Keyframe(0, 0);
        Frame[1] = new Keyframe(1, 1);
        DT.data.curve = new AnimationCurve(Frame);

        DT.data.tip = null;
        DT.data.root = null;
        DT.data.tipTarget = tip;
        DT.data.rootTarget = root;
    }

	public static void CreateSpine(BasisLocalPlayer player, GameObject parent, Transform hips, Transform[] spineJoints, Transform head, BasisBoneTrackedRole hipRole, out BasisSpineIKConstraint SpineIKConstraint, Vector3[] spineCurvature, float chainWeight = 1.0f, bool maintainSpineLength = true)
	{
		player.LocalBoneDriver.FindBone(out BasisLocalBoneControl hipControl, hipRole);

		var boneRole = CreateAndSetParent(parent.transform, $"Bone Role {hipRole.ToString()}");
		SpineIKConstraint = BasisHelpers.GetOrAddComponent<BasisSpineIKConstraint>(boneRole);

		// Set the transform references FIRST
		SpineIKConstraint.data.hips = hips;
		SpineIKConstraint.data.spineJoints = spineJoints;
		SpineIKConstraint.data.head = head;

		// Set control parameters
		SpineIKConstraint.data.chainWeight = chainWeight;
		SpineIKConstraint.data.maintainSpineLength = maintainSpineLength;

		// Initialize curvature array properly
		if (spineCurvature == null || spineCurvature.Length != spineJoints.Length)
		{
			spineCurvature = new Vector3[spineJoints.Length];
		}
		SpineIKConstraint.data.SpineCurvature = spineCurvature;

		// IMPORTANT: Manually calibrate the original distances after setting transforms
		CalibrateSpineDistances(ref SpineIKConstraint.data, hips, spineJoints);

		// Set target to head position/rotation
		SpineIKConstraint.data.headTargetPosition = head.position;
		SpineIKConstraint.data.headTargetRotation = head.rotation.eulerAngles;

		GenerateRequiredTransforms(player, head);
	}

	// Helper method to manually calibrate distances
	private static void CalibrateSpineDistances(ref BasisSpineIKConstraintData data, Transform hips, Transform[] spineJoints)
	{
		if (spineJoints == null || spineJoints.Length == 0) return;

		int jointCount = spineJoints.Length + 1; // Include hips
		data.m_OriginalDistances = new Vector3[jointCount];

		// Store original distances between consecutive joints
		data.m_OriginalDistances[0] = Vector3.zero; // Hips has no previous joint

		Transform prev = hips;
		for (int i = 0; i < spineJoints.Length; i++)
		{
			if (spineJoints[i] != null)
			{
				Vector3 offset = spineJoints[i].position - prev.position;
				data.m_OriginalDistances[i + 1] = offset;
				prev = spineJoints[i];
			}
		}
	}

	public static void CreateTwoBone(BasisLocalPlayer player, GameObject Parent, Transform root, Transform mid, Transform tip, BasisBoneTrackedRole TargetRole, BasisBoneTrackedRole BendRole, bool UseBoneRole, out BasisTwoBoneIKConstraint TwoBoneIKConstraint, bool maintainTargetPositionOffset, bool maintainTargetRotationOffset)
    {
        player.LocalBoneDriver.FindBone(out BasisLocalBoneControl TargetControl, TargetRole);

        GameObject BoneRole = CreateAndSetParent(Parent.transform, $"Bone Role {TargetRole.ToString()}");
        TwoBoneIKConstraint = BasisHelpers.GetOrAddComponent<BasisTwoBoneIKConstraint>(BoneRole);

        Vector3 PositionOffset = new Vector3(0, 0, 0);

        Quaternion RotationOffset =  tip.rotation;
        EnableTwoBoneIk(TwoBoneIKConstraint, PositionOffset, RotationOffset.eulerAngles);
        Quaternion Rotation = TargetControl.OutgoingWorldData.rotation;
        TwoBoneIKConstraint.data.TargetPosition = TargetControl.OutgoingWorldData.position;
        TwoBoneIKConstraint.data.TargetRotation = Rotation.eulerAngles;
        if (UseBoneRole)
        {
            if (player.LocalBoneDriver.FindBone(out BasisLocalBoneControl HintControl, BendRole))
            {
                Quaternion HintRotation = HintControl.OutgoingWorldData.rotation;
                TwoBoneIKConstraint.data.HintPosition = HintControl.OutgoingWorldData.position;
                TwoBoneIKConstraint.data.HintRotation = HintRotation.eulerAngles;
            }
        }
        TwoBoneIKConstraint.data.root = root;
        TwoBoneIKConstraint.data.mid = mid;
        TwoBoneIKConstraint.data.tip = tip;
        GenerateRequiredTransforms(player, tip);
    }

    public static void CreateTwoBoneHand(BasisLocalPlayer player, GameObject Parent, Transform root, Transform mid, Transform tip, BasisBoneTrackedRole TargetRole, BasisBoneTrackedRole BendRole, bool UseBoneRole, out BasisTwoBoneIKConstraintHand TwoBoneIKConstraint, bool maintainTargetPositionOffset, bool maintainTargetRotationOffset)
    {
        player.LocalBoneDriver.FindBone(out BasisLocalBoneControl TargetControl, TargetRole);

        GameObject BoneRole = CreateAndSetParent(Parent.transform, $"Bone Role {TargetRole.ToString()}");
        TwoBoneIKConstraint = BasisHelpers.GetOrAddComponent<BasisTwoBoneIKConstraintHand>(BoneRole);
        TwoBoneIKConstraint.data.M_CalibratedOffset = new Vector3(0, 0, 0);
        TwoBoneIKConstraint.data.M_CalibratedRotation = tip.rotation.eulerAngles;
        TwoBoneIKConstraint.data.TargetPosition = TargetControl.OutgoingWorldData.position;
        TwoBoneIKConstraint.data.TargetRotation = TargetControl.OutgoingWorldData.rotation.eulerAngles;

        if (UseBoneRole && player.LocalBoneDriver.FindBone(out BasisLocalBoneControl HintControl, BendRole))
        {
            Quaternion HintRotation = HintControl.OutgoingWorldData.rotation;
            TwoBoneIKConstraint.data.HintPosition = HintControl.OutgoingWorldData.position;
            TwoBoneIKConstraint.data.HintRotation = HintRotation.eulerAngles;
        }
  
        TwoBoneIKConstraint.data.root = root;
        TwoBoneIKConstraint.data.mid = mid;
        TwoBoneIKConstraint.data.tip = tip;
        GenerateRequiredTransforms(player, tip);
    }

    public static void WriteUpWeights(BasisLocalBoneControl Control, BasisApplyTranslation Constraint)
    {
        Control.WeightsChanged += (delegate (float positionWeight, float rotationWeight)
        {
            UpdateIKRig(positionWeight, rotationWeight, Constraint);
        });
    }

    public static void UpdateIKRig(float PositionWeight, float RotationWeight, BasisApplyTranslation Constraint)
    {
        Constraint.weight = PositionWeight;
    }

	public static void GenerateRequiredTransforms(BasisLocalPlayer player, Transform baseLevel)
	{
		if (baseLevel.parent == null || player.LocalRigDriver == null)
			return;

		Transform hipsTransform = player.LocalAvatarDriver?.References?.Hips;
		Transform currentTransform = baseLevel.parent;

		while (currentTransform != null && currentTransform != hipsTransform)
		{
			// Get or add RigTransform component
			RigTransform rigTransform = currentTransform.GetComponent<RigTransform>()
				?? currentTransform.gameObject.AddComponent<RigTransform>();

			// Add to collection if not already present
			if (!player.LocalRigDriver.AdditionalTransforms.Contains(rigTransform))
			{
				player.LocalRigDriver.AdditionalTransforms.Add(rigTransform);
			}

			currentTransform = currentTransform.parent;
		}
	}

	public static GameObject CreateAndSetParent(Transform parent, string name)
	{
		var boneName = $"Bone Role {name}";
		var existingChild = parent.Find(boneName);

		if (existingChild != null)
			return existingChild.gameObject;

		var newObject = new GameObject(name);
		newObject.transform.SetParent(parent);
		return newObject;
	}
}
