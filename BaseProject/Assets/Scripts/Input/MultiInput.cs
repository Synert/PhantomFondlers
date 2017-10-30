using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using XInputDotNetPure;

public class MultiInput : MonoBehaviour {

    //storage for keys
	[SerializeField]
	public List<keyInfo> keys = new List<keyInfo>();
	[SerializeField]
	public List<bool> visible = new List<bool>();

	ControllerPoll conPoll;

	void Start() {
		conPoll = GameObject.FindObjectOfType<ControllerPoll> ();
	}

	void Update() {
		foreach (keyInfo key in keys) {
			int currentKeyActivations = 0;
			for (int a = 0; a < key.keypad.Count; a++) {
				//grab current controller data
				controllerInputData conData = conPoll.ControllerData [(int)key.keypad[a].controller].conData;
				key.keypad [a].keyData.data.state = conData;
				for (int b = 0; b < key.keypad [a].selectedVariables.Length; b++) {
					if (currentKeyActivations < key.inputsAccepted) {
						if (key.keypad [a].selectedVariables [b] != button.none) {
							if (key.keypad [a].selectedVariables [b] == conData.controllerVariables [b]) {
								
								currentKeyActivations++;
								activate (key, 2, a);

							} else if (key.keypad [a].selectedVariables [b] == button.anyDown &&
							          (conData.controllerVariables [b] == button.pressed ||
							          conData.controllerVariables [b] == button.pressedFirstUpdate)) {

								currentKeyActivations++;
								activate (key, 2, a);

							} else if (key.keypad [a].selectedVariables [b] == button.anyUp &&
							          (conData.controllerVariables [b] == button.released ||
							          conData.controllerVariables [b] == button.releasedFirstUpdate)) {

								currentKeyActivations++;
								activate (key, 2, a);

							} else if (key.keypad [a].selectedVariables [b] == button.any) {

								currentKeyActivations++;
								activate (key, 2, a);

							}
						}
					}

				}
			}
			for (int a = 0; a < key.axis.Count; a++) {
				//do axis stuff here
				if (!key.axis [a].dualAxis) {

					if (currentKeyActivations < key.inputsAccepted) {
						//poll axis and test deadzone
						key.axis [a].keyData.data.axisValue = Input.GetAxisRaw (key.axis [a].axis);
						if (key.axis [a].keyData.data.axisValue < key.axis [a].axisDeadZone) {
							key.axis [a].keyData.data.axisValue = 0;
						}

						//activate if above is not 0
						if (key.axis [a].keyData.data.axisValue != 0) {
							currentKeyActivations++;
							activate (key, 0, a);
						}

					}
				} else {
					if (currentKeyActivations < key.inputsAccepted) {

						//poll axis and test deadzone
						key.axis [a].keyData.data.multiAxisValue.x = Input.GetAxisRaw (key.axis [a].axis);
						key.axis [a].keyData.data.multiAxisValue.y = Input.GetAxisRaw (key.axis [a].axis2);
						if (key.axis [a].keyData.data.multiAxisValue.magnitude < key.axis [a].axis2DeadZone) {
							key.axis [a].keyData.data.multiAxisValue = Vector2.zero;
						}

						//activate if either above is not 0
						if (key.axis [a].keyData.data.multiAxisValue != Vector2.zero) {
							currentKeyActivations++;
							activate (key, 0, a);
						}
					}
				}
			} //loop through all key buttons lists
			for (int a = 0; a < key.buttons.Count; a++) {
				//loop through all button list
				for (int b = 0; b < key.buttons [a].button.Count; b++) {
					//do button stuff here
					if (currentKeyActivations < key.inputsAccepted) {
						if ((Input.GetKey (key.buttons [a].button [b]) && key.buttons [a].keyType [b] == keyInputType.getKey) ||
						   (Input.GetKeyDown (key.buttons [a].button [b]) && key.buttons [a].keyType [b] == keyInputType.getKeyDown) ||
							(Input.GetKeyUp (key.buttons [a].button [b]) && key.buttons [a].keyType [b] == keyInputType.getKeyUp)) {
							currentKeyActivations++;
							activate (key, 1, a);
						}
					}
				}
			}
		}
	}

	void activate(keyInfo _key, int type, int index) {
		//deal with 1st layer pass on key
		basicKeyInfo key = new basicKeyInfo();
		if (type == 0) {
			key = _key.axis [index].keyData;
		} else if (type == 1) {
			key = _key.buttons [index].keyData;
		} else {
			key = _key.keypad [index].keyData;
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
		if (name != "") {
			info.SendMessage (name);
		}
	}

	void customVariableInvoke(GameObject info, string name, variableData variable) {
		if (name != "") {
			info.SendMessage (name, variable);
		}
	}
		
}

//default class for key input data
[System.Serializable]
public class keyInfo
{
	//basic variables for input names should explain it
	public int inputsAccepted = 1;

	public bool globalFunction;
	public bool globalFunctionObj;
	public bool globalController;

	public PlayerIndex globalControllerIndex;
	public GameObject globalFunctionObject;
	public string globalFunctionName;

	public List<axisInfo> axis;
	public List<buttonInfo> buttons;
	public List<controllerInfo> keypad;

}

[System.Serializable]
public class axisInfo {
	public string axis;
	public float axisDeadZone;
	public string axis2;
	public float axis2DeadZone;
	public bool dualAxis;
	public basicKeyInfo keyData;
}

[System.Serializable]
public class buttonInfo {
	public List<KeyCode> button;
	public List<keyInputType> keyType;
	public basicKeyInfo keyData;
}

[System.Serializable]
public class controllerInfo {
	public button[] selectedVariables = new button[18];
	public PlayerIndex controller;
	public basicKeyInfo keyData;
	public bool displayMainButtons;
	public bool displayMiscButtons;
	public bool displayDpadButtons;
	public bool displayVariable;
}

[System.Serializable]
public class basicKeyInfo {
	//basic data
	public bool variableInput;
	public bool expandInput;
	public bool viewable;
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

	//gamepad data
	public controllerInputData state;

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

[System.Serializable]
public class inputData {
	public float input;
}

[System.Serializable]
public class inputsData {
	public Vector2 inputs;
}

[System.Serializable]
public enum button {
	none,
	released,
	pressed,
	pressedFirstUpdate,
	releasedFirstUpdate,
	anyDown,
	anyUp,
	any
}