namespace UnityEngine.Animations.Rigging
{
    public struct BasisFullSpineIKData
    {
        public AnimationStream stream;

        public ReadWriteTransformHandle avatarHead;
		public ReadWriteTransformHandle avatarNeck;
		public ReadWriteTransformHandle avatarUpperChest;
		public ReadWriteTransformHandle avatarChest;
		public ReadWriteTransformHandle avatarSpine;
		public ReadWriteTransformHandle avatarHips;

        public AffineTransform headTarget;
        public AffineTransform neckTarget;
		public AffineTransform upperChestTarget;
		public AffineTransform chestTarget;
		public AffineTransform spineTarget;
		public AffineTransform hipsTarget;

		public float maxSpineLength;
		public float maxNeckLength;
	}

    public static class BasisFullSpineIKSolver
	{
        public static void Solve(BasisFullSpineIKData data)
        {
			// Make sure spine length doesn't exceed max length
			var spineVector = data.headTarget.translation - data.hipsTarget.translation;

			var calculatedHeadPosition = Mathf.Abs(Vector3.Magnitude(spineVector)) > data.maxSpineLength ?
				data.hipsTarget.translation + (spineVector.normalized * data.maxSpineLength) :
				data.headTarget.translation;

			data.avatarHead.SetPosition(data.stream, calculatedHeadPosition);

			// Rotation always hard set to camera rotation.
			data.avatarHead.SetRotation(data.stream, data.headTarget.rotation);

			// Calculate neck position first
			var neckVector = data.neckTarget.translation - calculatedHeadPosition;
			var neckVectorMagnitude = Mathf.Abs(Vector3.Magnitude(neckVector));
			var calculatedNeckPosition = data.neckTarget.translation;

			if(neckVectorMagnitude > data.maxNeckLength)
			{
				//TODO: Calculations and shit
			}

			data.avatarNeck.SetPosition(data.stream, calculatedNeckPosition);

			// Calculate neck rotation second
			data.avatarNeck.SetRotation(data.stream, data.neckTarget.rotation);

			// Calculate upperChest position first
			data.avatarUpperChest.SetPosition(data.stream, data.upperChestTarget.translation);

			// Calculate upperChest rotation second
			data.avatarUpperChest.SetRotation(data.stream, data.upperChestTarget.rotation);

			// Calculate chest position first
			data.avatarChest.SetPosition(data.stream, data.chestTarget.translation);

			// Calculate chest rotation second
			data.avatarChest.SetRotation(data.stream, data.chestTarget.rotation);

			// Calculate spine position first
			data.avatarSpine.SetPosition(data.stream, data.spineTarget.translation);

			// Calculate spine rotation second
			data.avatarSpine.SetRotation(data.stream, data.spineTarget.rotation);

			// Always set the head to whatever the camera/headset is doing
			data.avatarHips.SetPosition(data.stream, data.hipsTarget.translation);
			data.avatarHips.SetRotation(data.stream, data.hipsTarget.rotation);

		}
    }
}
