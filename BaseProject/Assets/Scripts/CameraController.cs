using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public List<GameObject> objects = new List<GameObject>();
	public float offset;
	public float cameraZAxis;
	public Vector2 cameraSpeedIn;
	public Vector2 cameraSpeedOut;
	public float prevTargetSize;
	public Vector3 prevTargetPos;

	void Start () {
		//grab all objects in scene
		PlayerController[] temp = GameObject.FindObjectsOfType<PlayerController> ();
		foreach (PlayerController obj in temp) {
			objects.Add (obj.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		//setup basic stats
		Vector3 topLeft = Vector3.one * Mathf.Infinity;
		topLeft.y = Mathf.NegativeInfinity;
		topLeft.z = cameraZAxis;
		Vector3 bottomRight = Vector3.one * Mathf.NegativeInfinity;
		bottomRight.y = Mathf.Infinity;
		bottomRight.z = cameraZAxis;

		//loop through all objects and setup basic stats based on their positions
		//and distances to each other
		foreach (GameObject obj in objects) {
			if (obj) {
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
		}
			
		Debug.DrawLine (topLeft, bottomRight, Color.red);

		//setup basic data from above stats
		Vector3 positionCur = transform.position;
		float sizeCur = Camera.main.orthographicSize;
		Vector3 targetPos = topLeft + ((bottomRight - topLeft) / 2);
		float targetSize = ((bottomRight - topLeft) / 2).magnitude + offset;

		//move camera pos to new camera pos
		if (targetPos.x - positionCur.x > 0.1f || targetPos.x - positionCur.x < -0.1f ) {
			if (positionCur.x > targetPos.x) {
				positionCur.x = Mathf.Lerp (positionCur.x, targetPos.x, Time.deltaTime * cameraSpeedOut.x);
			} else {
				positionCur.x = Mathf.Lerp (positionCur.x, targetPos.x, Time.deltaTime * cameraSpeedIn.x);
			}
		}

		//move camera y pos to new camera pos
		if (targetPos.y - positionCur.y > 0.1f || targetPos.y - positionCur.y < -0.1f) {
			if (positionCur.y > targetPos.y) {
				positionCur.y = Mathf.Lerp (positionCur.y, targetPos.y, Time.deltaTime * cameraSpeedOut.x);
			} else {
				positionCur.y = Mathf.Lerp (positionCur.y, targetPos.y, Time.deltaTime * cameraSpeedIn.x);
			}
		}

		//change camera size to fit all objects
		if (targetSize - sizeCur > 0.1f || targetSize - sizeCur < -0.1f) {
			if (targetSize < sizeCur) {
				sizeCur = Mathf.Lerp (sizeCur, targetSize, Time.deltaTime * cameraSpeedIn.y);
			} else {
				sizeCur = Mathf.Lerp (sizeCur, targetSize, Time.deltaTime * cameraSpeedOut.y);
			}
		}
			
		//set data
		transform.position = positionCur;
		Camera.main.orthographicSize = sizeCur;

		prevTargetPos = targetPos;
		prevTargetSize = sizeCur;
	}
}
