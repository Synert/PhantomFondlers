using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ControllerPoll : MonoBehaviour {

	public bool[] Controllers = { false, false, false, false };
	public controllerButtonData[] ControllerData = new controllerButtonData[4];

	// Update is called once per frame
	void Update () {
		for (int a = 0; a < 4; a++) {
			GamePadState con = GamePad.GetState ((PlayerIndex)a, GamePadDeadZone.Circular);
			if (con.IsConnected) {
				Controllers [a] = true;
				ControllerData [a].UpdateKeys (con);
			} else {
				Controllers [a] = false;
			}
		}
	}
}

[System.Serializable]
public class controllerInputData {
	//default data
	public button[] controllerVariables = new button[18];

	public inputsData ThumbStickLeft;
	public inputsData ThumbStickRight;
	public inputData TriggerLeft;
	public inputData TriggerRight;
}

[System.Serializable]
public class controllerButtonData {
	//default data
	public controllerInputData conData;

	public GamePadState previousState;

		public void UpdateKeys(GamePadState state) {

		//update inputs
		testButton (state.Buttons.A, previousState.Buttons.A, out conData.controllerVariables [0]);
		testButton (state.Buttons.B, previousState.Buttons.B, out conData.controllerVariables [1]);
		testButton (state.Buttons.X, previousState.Buttons.X, out conData.controllerVariables [2]);
		testButton (state.Buttons.Y, previousState.Buttons.Y, out conData.controllerVariables [3]);
		testButton (state.Buttons.Guide, previousState.Buttons.Guide, out conData.controllerVariables [4]);
		testButton (state.Buttons.Start, previousState.Buttons.Start, out conData.controllerVariables [5]);
		testButton (state.Buttons.RightStick, previousState.Buttons.RightStick, out conData.controllerVariables [6]);
		testButton (state.Buttons.RightShoulder, previousState.Buttons.RightShoulder, out conData.controllerVariables [7]);
		testButton (state.Buttons.LeftStick, previousState.Buttons.LeftStick, out conData.controllerVariables [8]);
		testButton (state.Buttons.LeftShoulder, previousState.Buttons.LeftShoulder, out conData.controllerVariables [9]);
		testButton (state.DPad.Up, previousState.DPad.Up, out conData.controllerVariables [10]);
		testButton (state.DPad.Down, previousState.DPad.Down, out conData.controllerVariables [11]);
		testButton (state.DPad.Left, previousState.DPad.Left, out conData.controllerVariables [12]);
		testButton (state.DPad.Right, previousState.DPad.Right, out conData.controllerVariables [13]);

		//setup thumb and test input
		conData.ThumbStickLeft.inputs = new Vector2 (state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
		testVector2Button (new Vector2 (state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y), 
			new Vector2 (previousState.ThumbSticks.Left.X, previousState.ThumbSticks.Left.Y), out conData.controllerVariables [14]);

		conData.ThumbStickRight.inputs = new Vector2 (state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
		testVector2Button (new Vector2 (state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y), 
			new Vector2 (previousState.ThumbSticks.Right.X, previousState.ThumbSticks.Right.Y), out conData.controllerVariables [15]);

		//setup trigger and test input
		conData.TriggerLeft.input = state.Triggers.Left;
		testFloatButton (state.Triggers.Left, previousState.Triggers.Left, out conData.controllerVariables [16]);

		conData.TriggerRight.input = state.Triggers.Right;
		testFloatButton (state.Triggers.Right, previousState.Triggers.Right, out conData.controllerVariables [17]);

		previousState = state;
	}

	void testButton(ButtonState testAgainst, ButtonState prevTestAgainst, out button valueSet) {
		if (testAgainst == ButtonState.Pressed) {
			if (prevTestAgainst == ButtonState.Released) {
				valueSet = button.pressedFirstUpdate;
			} else {
				valueSet = button.pressed;
			}
		} else {
			if (prevTestAgainst == ButtonState.Pressed) {
				valueSet = button.releasedFirstUpdate;
			} else {
				valueSet = button.released;
			}
		}
	}

	void testFloatButton(float testAgainst, float prevTestAgainst, out button valueSet) {
		if (testAgainst != 0) {
			if (prevTestAgainst == 0) {
				valueSet = button.pressedFirstUpdate;
			} else {
				valueSet = button.pressed;
			}
		} else {
			if (prevTestAgainst != 0) {
				valueSet = button.releasedFirstUpdate;
			} else {
				valueSet = button.released;
			}
		}
	}

	void testVector2Button(Vector2 testAgainst, Vector2 prevTestAgainst, out button valueSet) {
		if (testAgainst != Vector2.zero) {
			if (prevTestAgainst == Vector2.zero) {
				valueSet = button.pressedFirstUpdate;
			} else {
				valueSet = button.pressed;
			}
		} else {
			if (prevTestAgainst != Vector2.zero) {
				valueSet = button.releasedFirstUpdate;
			} else {
				valueSet = button.released;
			}
		}
	}

}