using UnityEngine;

public class IKTest : MonoBehaviour
{
    public Transform head;
    public Transform root;
    public Transform mid;

	public Vector3 lastHeadTransform;

	private void Update()
	{
		if (head.position == lastHeadTransform) return;

		var ab = Vector3.Magnitude(head.position - mid.position);
		var ac = Vector3.Magnitude(head.position - root.position);
		var bc = Vector3.Magnitude(mid.position - root.position);

		var theta = Mathf.Atan((root.position.z - head.position.z) / (root.position.x - head.position.x));
	}
}
