using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public List<GameObject> objects = new List<GameObject>();
	public float offset;
	public Vector2 cameraSpeedIn;
	public Vector2 cameraSpeedOut;
	public float prevTargetSize;
	public Vector3 prevTargetPos;

	void Start () {
		PlayerController[] temp = GameObject.FindObjectsOfType<PlayerController> ();
		foreach (PlayerController obj in temp) {
			objects.Add (obj.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		Vector3 topLeft = Vector3.one * Mathf.Infinity;
		topLeft.y = Mathf.NegativeInfinity;
		topLeft.z = -10;
		Vector3 bottomRight = Vector3.one * Mathf.NegativeInfinity;
		bottomRight.y = Mathf.Infinity;
		bottomRight.z = -10;
		foreach (GameObject obj in objects) {
			if (obj.transform.position.x < topLeft.x) {
				topLeft.x = obj.transform.position.x;
			}
			if (obj.transform.position.y > topLeft.y) {
				topLeft.y = obj.transform.position.y;
			}

			if (obj.transform.position.x > bottomRight.x) {
				bottomRight.x = obj.transform.position.x;
			}
			if (obj.transform.position.y < bottomRight.y) {
				bottomRight.y = obj.transform.position.y;
			}
		}

		Debug.DrawLine (topLeft, bottomRight, Color.red);
		Vector3 positionCur = transform.position;
		float sizeCur = Camera.main.orthographicSize;
		Vector3 targetPos = topLeft + ((bottomRight - topLeft) / 2);
		float targetSize = ((bottomRight - topLeft) / 2).magnitude + offset;

		if (targetPos.x - positionCur.x > 0.1f || targetPos.x - positionCur.x < -0.1f ) {
			if (positionCur.x > targetPos.x) {
				positionCur.x -= Time.deltaTime * cameraSpeedOut.x;
			} else {
				positionCur.x += Time.deltaTime * cameraSpeedIn.x;
			}
		}

		if (targetPos.y - positionCur.y > 0.1f || targetPos.y - positionCur.y < -0.1f) {
			if (positionCur.y > targetPos.y) {
				positionCur.y -= Time.deltaTime * cameraSpeedOut.x;
			} else {
				positionCur.y += Time.deltaTime * cameraSpeedIn.x;
			}
		}

		if (targetSize - sizeCur > 0.1f || targetSize - sizeCur < -0.1f) {
			if (targetSize < sizeCur) {
				sizeCur -= Time.deltaTime * cameraSpeedIn.y;
			} else {
				sizeCur += Time.deltaTime * cameraSpeedOut.y;
			}
		}
			
		transform.position = positionCur;
		Camera.main.orthographicSize = sizeCur;

		prevTargetPos = targetPos;
		prevTargetSize = sizeCur;
	}
}
