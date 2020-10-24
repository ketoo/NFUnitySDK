using System.Collections;
using UnityEngine;


namespace NFrame
{
	public class CameraViewController : MonoBehaviour
	{
		#region 数据成员

		public Transform Target;
		private float distance = 2.0f;
		private Vector2 euler;

		private Quaternion targetRot;
		private Vector3 targetLookAt;
		private float targetDist;
		private Vector3 distanceVec = new Vector3(0, 0, 0);
		private Transform target;
		#endregion

		#region Start/Update/FixedUpdate
		public void Start()
		{
			Vector3 angles = transform.eulerAngles;
			euler.x = angles.y;
			euler.y = angles.x;
			//unity only returns positive euler angles but we need them in -90 to 90  
			euler.y = Mathf.Repeat(euler.y + 180f, 360f) - 180f;

			GameObject go = new GameObject("_FreeCameraTarget");
			go.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
			go.transform.position = Target.position;
			go.transform.rotation = Target.rotation;
			target = go.transform;
			targetDist = (transform.position - target.position).magnitude;

			targetRot = transform.rotation;
			targetLookAt = target.position;
		}

		public void Update()
		{
			if (target)
			{
				float dx = Input.GetAxis("Mouse X");
				float dy = Input.GetAxis("Mouse Y");

				bool click1 = Input.GetMouseButton(1) || Input.GetMouseButton(0);
				bool click2 = Input.GetMouseButton(2);

				if (click2)
				{
					dx = dx * 10 * 0.005f * targetDist;
					dy = dy * 10 * 0.005f * targetDist;
					targetLookAt -= transform.up * dy + transform.right * dx;
				}

				else if (click1)
				{
					dx = dx * 250 * 0.02f;
					dy = dy * 120 * 0.02f;
					euler.x += dx;
					euler.y -= dy;
					targetRot = Quaternion.Euler(euler.y, euler.x, 0);
				}

				targetDist -= Input.GetAxis("Mouse ScrollWheel") * 30 * 0.5f;
			}
		}

		public void FixedUpdate()
		{
			distance = 0.7f * targetDist + (1 - 0.7f) * distance;

			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.5f);
			target.position = Vector3.Lerp(target.position, targetLookAt, 0.7f);
			distanceVec.z = distance;
			transform.position = target.position - transform.rotation * distanceVec;
		}
		#endregion
	}
}