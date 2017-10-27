using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class MultiInput : MonoBehaviour {

    //storage for keys
	[SerializeField]
	public List<keyInfo> keys = new List<keyInfo>();
	[SerializeField]
	public List<bool> visible = new List<bool>();

	void Update() {
		foreach (keyInfo key in keys) {
			for (int a = 0; a < key.axis.Count; a++) {
				//do axis stuff here
				if (!key.axis [a].multiAxis) {
					key.axis [a].keyData.data.axisValue = Input.GetAxis (key.axis [a].axis);
					if (key.axis [a].keyData.data.axisValue != 0) {
						activate (key, true, a);
					}
				} else {
					key.axis [a].keyData.data.multiAxisValue.x = Input.GetAxis (key.axis [a].axis);
					key.axis [a].keyData.data.multiAxisValue.y = Input.GetAxis (key.axis [a].axis2);
					if (key.axis [a].keyData.data.multiAxisValue != Vector2.zero) {
						activate (key, true, a);
					}
				}
			}
			for (int a = 0; a < key.buttons.Count; a++) {
				//do button stuff here
				if ((Input.GetKey (key.buttons[a].button) && key.buttons[a].keyType == keyInputType.getKey) ||
					(Input.GetKeyDown (key.buttons[a].button) && key.buttons[a].keyType == keyInputType.getKeyDown) ||
					(Input.GetKeyUp (key.buttons[a].button) && key.buttons[a].keyType == keyInputType.getKeyUp)) {
					activate(key, false, a);
				}
			}
		}
	}

	void activate(keyInfo _key, bool type, int index) {
		//deal with 1st layer pass on key
		basicKeyInfo key = new basicKeyInfo();
		if (type) {
			key = _key.axis [index].keyData;
		} else {
			key = _key.buttons [index].keyData;
		}

		//check if key has a set function
		if (key.function) {
			//check if the key has an object
			if (key.functionObject) {
				//check if the key has variable input
				if (key.variableInput) {
					//carry our custom function
					customVariableInvoke (key.functionObject, key.functionName, key.data);
				} else {
					//carry our custom function
					customInvoke (key.functionObject, key.functionName);
				}
			} else {
				Debug.Log ("You need to add an object to activate this key: " + key.functionName);
			}
		} else {
			//check if key has public fuction
			if (key.publicFunction != null) {
				//invoke unity event
				key.publicFunction.Invoke ();
			} else {
				Debug.Log ("You need to add an event to activate this key: " + gameObject.name);
			}
		}
	}

	void customInvoke(GameObject info, string name) {
		info.SendMessage (name);
	}

	void customVariableInvoke(GameObject info, string name, variableData variable) {
		info.SendMessage (name, variable);
	}

}

//default class for key input data
[System.Serializable]
public class keyInfo
{
	//basic variables for input names should explain it
	public int keyInputsAccepted;
	public bool globalFunction;
	public bool globalFunctionObj;
	public GameObject globalFunctionObject;
	public string globalFunctionName;
	public List<axisInfo> axis;
	public List<buttonInfo> buttons;

}

[System.Serializable]
public class axisInfo {
	public string axis;
	public string axis2;
	public bool multiAxis;
	public basicKeyInfo keyData;
}

[System.Serializable]
public class buttonInfo {
	public KeyCode button;
	public basicKeyInfo keyData;
	public keyInputType keyType;
}

[System.Serializable]
public class basicKeyInfo {
	//basic data
	public bool variableInput;
	public bool expandInput;
	public variableData data;

	//deal with private functions
	public bool function;
	public string functionName;
	public GameObject functionObject;

	//deal with public functions
	public UnityEvent publicFunction;
}

[System.Serializable]
public class variableData {
	//axis data
	public float axisValue;
	public Vector2 multiAxisValue;

	//all data types needed
	public bool b;
	public byte by;
	public char c;
	public float f;
	public int i;
	public GameObject obj;
	public MonoBehaviour sc;
	public string s;
	public Transform t;
	public UnityEvent u;
	public Vector2 v2;
	public Vector3 v3;
	public Vector4 v4;
}

public enum keyInputType {
	getKey, getKeyDown, getKeyUp
}