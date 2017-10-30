using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using XInputDotNetPure;

//create custom editor for multi-input
[CustomEditor(typeof(MultiInput))]
[CanEditMultipleObjects]
public class MultiInputEditor : Editor {

	SerializedProperty keys;
	SerializedProperty visible;

	void OnEnable() {
		keys = serializedObject.FindProperty("keys");
		visible = serializedObject.FindProperty ("visible");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		//begin whole button gui
		GUI.backgroundColor = new Color(0.2f,0.2f,0.5f,1);
		GUILayout.BeginVertical(EditorStyles.helpBox);
		GUI.backgroundColor = new Color(0.9f,0.9f,0.9f,0.5f);
		//GUI.backgroundColor = Color.white;

        //display count of key data
		EditorGUILayout.IntField("Buttons", keys.arraySize);

        //display add/remove buttons for key data
		GUILayout.BeginHorizontal(EditorStyles.helpBox);

		GUILayout.FlexibleSpace ();
		if (GUILayout.Button("Add", GUILayout.Width(250)))
        {
            //add new blank key data
			keys.InsertArrayElementAtIndex (keys.arraySize);
			visible.InsertArrayElementAtIndex (visible.arraySize);
        }
		if (GUILayout.Button("Remove", GUILayout.Width(250)))
        {
            //check if key data has data
			if (keys.arraySize > 0)
            {
				//remove last key data
				keys.DeleteArrayElementAtIndex(keys.arraySize-1);
				visible.DeleteArrayElementAtIndex(visible.arraySize-1);
            }
		}
		GUILayout.FlexibleSpace ();

		serializedObject.ApplyModifiedProperties ();

		GUILayout.EndHorizontal();
		GUILayout.Space(10);

        //loop through each key data and display change options
		for (int a = 0; a < keys.arraySize; a++) {
			//check if current key is visible
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			if (visible.GetArrayElementAtIndex (a).boolValue) {
				//grab current keyInfo
				SerializedProperty currentKeySet = keys.GetArrayElementAtIndex (a);

				//grab the current keysets
				SerializedProperty currentKeyButtonList = currentKeySet.FindPropertyRelative ("buttons");
				SerializedProperty currentKeyAxisList = currentKeySet.FindPropertyRelative ("axis");
				SerializedProperty currentKeyGamepadList = currentKeySet.FindPropertyRelative ("keypad");

				//allow closing of element data
				GUI.backgroundColor = new Color (1, 1, 1, 1);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				GUILayout.FlexibleSpace ();
				serializedObject.Update ();
				if (GUILayout.Button ("Close Element " + a.ToString (), GUILayout.Width (300))) {
					visible.GetArrayElementAtIndex (a).boolValue = false;
				}
				serializedObject.ApplyModifiedProperties ();
				GUILayout.FlexibleSpace ();
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);
				GUILayout.EndHorizontal ();

				//start data section
				GUILayout.BeginVertical (EditorStyles.helpBox);
				serializedObject.Update ();

				//grab current global options
				SerializedProperty globalFunction = currentKeySet.FindPropertyRelative ("globalFunction");
				SerializedProperty globalFunctionObj = currentKeySet.FindPropertyRelative ("globalFunctionObj");
				SerializedProperty globalFunctionObject = currentKeySet.FindPropertyRelative ("globalFunctionObject");
				SerializedProperty globalFunctionName = currentKeySet.FindPropertyRelative ("globalFunctionName");

				GUILayout.BeginHorizontal ();
				EditorGUILayout.PropertyField (globalFunction);
				EditorGUILayout.PropertyField (globalFunctionObj);
				GUILayout.EndHorizontal ();

				if (globalFunction.boolValue) {
					EditorGUILayout.PropertyField (globalFunctionName);
				}
				if (globalFunctionObj.boolValue) {
					EditorGUILayout.PropertyField (globalFunctionObject);
				}

				EditorGUILayout.PropertyField (currentKeySet.FindPropertyRelative ("inputsAccepted"));
					
				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				GUILayout.FlexibleSpace ();
				//display adding for multiple axis
				if (GUILayout.Button ("Add Axis", GUILayout.Width (150))) {
					//add new blank key data
					currentKeyAxisList.InsertArrayElementAtIndex (currentKeyAxisList.arraySize);
				}
				if (GUILayout.Button ("Remove Axis", GUILayout.Width (150))) {
					//check if key data has data
					if (currentKeyAxisList.arraySize > 0) {
						//remove last key data
						currentKeyAxisList.DeleteArrayElementAtIndex (currentKeyAxisList.arraySize - 1);
					}
				}
				GUILayout.FlexibleSpace ();

				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);

				GUILayout.EndVertical ();

				serializedObject.ApplyModifiedProperties ();
				GUI.backgroundColor = new Color (0.25f,0.25f,0.25f,1);
				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);

				//loop through current Axis list
				serializedObject.Update ();
				for (int b = 0; b < currentKeyAxisList.arraySize; b++) {
					GUILayout.BeginVertical (EditorStyles.helpBox);
					serializedObject.Update ();
					//grab data
					SerializedProperty currentKey = currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("keyData");

					//setup multi/single axis support
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("dualAxis"));
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axis"));
						EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axisDeadZone"));

					if (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("dualAxis").boolValue) {
						EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axis2"));
						EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axis2DeadZone"));
					}

					//check if global vars
					if (globalFunction.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionName").stringValue = globalFunctionName.stringValue;
					} else {
						//if not display options for changing
						EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("function"));
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionName"));
						} else {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("publicFunction"));
						}
					}

					//check if global vars
					if (globalFunctionObj.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionObject").objectReferenceValue = globalFunctionObject.objectReferenceValue;
					} else {
						//if not display options for changing
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionObject"));
						}
					}

					//display options for changing variable
					EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("variableInput"));
					if (currentKey.FindPropertyRelative ("expandInput").boolValue) {
						//allow hiding of data block
						if (GUILayout.Button ("Hide Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
						//display data block
						if (currentKey.FindPropertyRelative ("variableInput").boolValue) {
							displayKeyData (currentKey);
						} else {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
					} else {
						//allow showing of data block
						if (GUILayout.Button ("Display Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = true;
						}
					}
					serializedObject.ApplyModifiedProperties ();
					EditorGUILayout.EndVertical ();
					GUILayout.Space (2);
				}
				serializedObject.ApplyModifiedProperties ();

				GUILayout.EndVertical ();
				GUILayout.Space (2);

				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				serializedObject.Update ();
				GUILayout.FlexibleSpace ();
				//display adding for multiple keys
				if (GUILayout.Button ("Add Button", GUILayout.Width (150))) {
					//add new blank key data
					currentKeyButtonList.InsertArrayElementAtIndex (currentKeyButtonList.arraySize);
				}
				if (GUILayout.Button ("Remove Button", GUILayout.Width (150))) {
					//check if key data has data
					if (currentKeyButtonList.arraySize > 0) {
						//remove last key data
						currentKeyButtonList.DeleteArrayElementAtIndex (currentKeyButtonList.arraySize - 1);
					}
				}
				serializedObject.ApplyModifiedProperties ();
				GUILayout.FlexibleSpace ();

				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);

				GUI.backgroundColor = new Color (0.25f,0.25f,0.25f,1);
				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);

				serializedObject.Update ();
				for (int b = 0; b < currentKeyButtonList.arraySize; b++) {
					EditorGUILayout.BeginVertical (EditorStyles.helpBox);
					serializedObject.Update ();

					SerializedProperty currentKeyButtonKey = currentKeyButtonList.GetArrayElementAtIndex (b);

					GUILayout.BeginHorizontal (EditorStyles.helpBox);

					serializedObject.Update ();
					GUILayout.FlexibleSpace ();
					//display adding for multiple keys
					if (GUILayout.Button ("Add Key", GUILayout.Width (150))) {
						//add new blank key data
						currentKeyButtonKey.FindPropertyRelative ("button").InsertArrayElementAtIndex (currentKeyButtonKey.FindPropertyRelative ("button").arraySize);
						currentKeyButtonKey.FindPropertyRelative ("keyType").InsertArrayElementAtIndex (currentKeyButtonKey.FindPropertyRelative ("keyType").arraySize);
					}
					if (GUILayout.Button ("Remove Key", GUILayout.Width (150))) {
						//check if key data has data
						if (currentKeyButtonKey.FindPropertyRelative ("button").arraySize > 0) {
							//remove last key data
							currentKeyButtonKey.FindPropertyRelative ("button").DeleteArrayElementAtIndex (currentKeyButtonKey.FindPropertyRelative ("button").arraySize - 1);
							currentKeyButtonKey.FindPropertyRelative ("keyType").DeleteArrayElementAtIndex (currentKeyButtonKey.FindPropertyRelative ("keyType").arraySize - 1);
						}
					}
					serializedObject.ApplyModifiedProperties ();
					GUILayout.FlexibleSpace ();

					//split up the options
					GUILayout.EndHorizontal ();

					for (int c = 0; c < currentKeyButtonKey.FindPropertyRelative ("button").arraySize; c++) {

						GUILayout.BeginHorizontal ();

						//grab data
						EditorGUILayout.LabelField("Key " + c, GUILayout.Width(50));
						EditorGUILayout.PropertyField (currentKeyButtonList.GetArrayElementAtIndex (b).FindPropertyRelative ("button").GetArrayElementAtIndex(c), GUIContent.none);
						EditorGUILayout.PropertyField (currentKeyButtonList.GetArrayElementAtIndex (b).FindPropertyRelative ("keyType").GetArrayElementAtIndex(c), GUIContent.none);

						GUILayout.EndHorizontal ();

					}

					SerializedProperty currentKey = currentKeyButtonList.GetArrayElementAtIndex (b).FindPropertyRelative ("keyData");

					//check if global vars
					if (globalFunction.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionName").stringValue = globalFunctionName.stringValue;
					} else {
						//if not display options for changing
						EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("function"));
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionName"));
						} else {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("publicFunction"));
						}
					}

					//check if global vars
					if (globalFunctionObj.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionObject").objectReferenceValue = globalFunctionObject.objectReferenceValue;
					} else {
						//if not display options for changing
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionObject"));
						}
					}

					//display options for changing variable
					EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("variableInput"));
					if (currentKey.FindPropertyRelative ("expandInput").boolValue) {
						//allow hiding of data block
						if (GUILayout.Button ("Hide Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
						//display data block
						if (currentKey.FindPropertyRelative ("variableInput").boolValue) {
							displayKeyData (currentKey);
						} else {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
					} else {
						//allow showing of data block
						if (GUILayout.Button ("Display Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = true;
						}
					}
					serializedObject.ApplyModifiedProperties ();
					EditorGUILayout.EndVertical ();
					GUILayout.Space (10);
				}
				serializedObject.ApplyModifiedProperties ();

				GUILayout.EndVertical ();
				GUILayout.EndVertical ();

				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				GUILayout.FlexibleSpace ();

				serializedObject.Update ();
				//display adding for multiple axxis
				if (GUILayout.Button ("Add GamePad", GUILayout.Width (150))) {
					//add new blank key data
					currentKeyGamepadList.InsertArrayElementAtIndex (currentKeyGamepadList.arraySize);
				}
				if (GUILayout.Button ("Remove GamePad", GUILayout.Width (150))) {
					//check if key data has data
					if (currentKeyGamepadList.arraySize > 0) {
						//remove last key data
						currentKeyGamepadList.DeleteArrayElementAtIndex (currentKeyGamepadList.arraySize - 1);
					}
				}
				serializedObject.ApplyModifiedProperties ();

				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();


				GUI.backgroundColor = new Color (0.25f,0.25f,0.25f,1);
				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);

				for (int b = 0; b < currentKeyGamepadList.arraySize; b++) {
					serializedObject.Update ();

					EditorGUILayout.BeginVertical (EditorStyles.helpBox);

					EditorGUILayout.PropertyField(currentKeyGamepadList.GetArrayElementAtIndex (b).FindPropertyRelative ("controller"));
					SerializedProperty state = currentKeyGamepadList.GetArrayElementAtIndex (b);
					SerializedProperty currentKey = currentKeyGamepadList.GetArrayElementAtIndex (b).FindPropertyRelative ("keyData");

					//check if global vars
					if (globalFunction.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionName").stringValue = globalFunctionName.stringValue;
					} else {
						//if not display options for changing
						EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("function"));
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionName"));
						} else {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("publicFunction"));
						}
					}

					//check if global vars
					if (globalFunctionObj.boolValue) {
						//set global vars to required
						currentKey.FindPropertyRelative ("function").boolValue = true;
						currentKey.FindPropertyRelative ("functionObject").objectReferenceValue = globalFunctionObject.objectReferenceValue;
					} else {
						//if not display options for changing
						if (currentKey.FindPropertyRelative ("function").boolValue) {
							EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionObject"));
						}
					}


					displayControllerInput (state);

					//display options for changing variable
					EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("variableInput"));
					if (currentKey.FindPropertyRelative ("expandInput").boolValue) {
						//allow hiding of data block
						if (GUILayout.Button ("Hide Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
						//display data block
						if (currentKey.FindPropertyRelative ("variableInput").boolValue) {
							displayKeyData (currentKey);
						} else {
							currentKey.FindPropertyRelative ("expandInput").boolValue = false;
						}
					} else {
						//allow showing of data block
						if (GUILayout.Button ("Display Variables")) {
							currentKey.FindPropertyRelative ("expandInput").boolValue = true;
						}
					}

					EditorGUILayout.EndVertical ();
					GUILayout.Space (10);

					serializedObject.ApplyModifiedProperties ();
				}

				GUILayout.EndVertical ();
				GUILayout.EndVertical ();
				GUILayout.EndVertical ();


			} else {
				
				//allow opening of element
				GUI.backgroundColor = new Color (1, 1, 1, 1);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				GUILayout.Space (100);
				if (GUILayout.Button ("Show Element " + a.ToString (), GUILayout.Width (300))) {
					visible.GetArrayElementAtIndex (a).boolValue = true;
				}
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);
				GUILayout.EndHorizontal ();
				GUILayout.Space (10);
			}

			GUILayout.EndVertical ();
		}
			
		GUILayout.EndVertical();
		
		serializedObject.ApplyModifiedProperties ();
	}

	void displayControllerInput(SerializedProperty state) {
		SerializedProperty selectedVars = state.FindPropertyRelative ("selectedVariables");
		SerializedProperty varLocation = state.FindPropertyRelative ("keyData").FindPropertyRelative ("data").FindPropertyRelative ("state").FindPropertyRelative ("controllerVariables");

		//make sure arrays are correct size
		while (varLocation.arraySize < 18) {
			varLocation.InsertArrayElementAtIndex (varLocation.arraySize);
		}

		while (selectedVars.arraySize < 18) {
			selectedVars.InsertArrayElementAtIndex (selectedVars.arraySize);
		}

		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		if (state.FindPropertyRelative ("displayMainButtons").boolValue) {
			if (GUILayout.Button ("Close Main Buttons")) {
				state.FindPropertyRelative ("displayMainButtons").boolValue = false;
			}
			labelWithProperty ("Xbox A", 140, varLocation.GetArrayElementAtIndex(0), selectedVars.GetArrayElementAtIndex (0), true, true, 2);

			labelWithProperty ("Xbox X", 140, varLocation.GetArrayElementAtIndex(1), selectedVars.GetArrayElementAtIndex (1), true, true, 2);

			labelWithProperty ("Xbox B", 140, varLocation.GetArrayElementAtIndex(2), selectedVars.GetArrayElementAtIndex (2), true, true, 2);

			labelWithProperty ("Xbox Y", 140, varLocation.GetArrayElementAtIndex(3), selectedVars.GetArrayElementAtIndex (3), true, true, 2);
		} else {
			if (GUILayout.Button ("Display Main Buttons")) {
				state.FindPropertyRelative ("displayMainButtons").boolValue = true;
			}
		}

		EditorGUILayout.EndVertical ();
		GUILayout.Space (2);
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		if (state.FindPropertyRelative ("displayMiscButtons").boolValue) {
			if (GUILayout.Button ("Close Misc Buttons")) {
				state.FindPropertyRelative ("displayMiscButtons").boolValue = false;
			}
			labelWithProperty ("Xbox Guide", 140, varLocation.GetArrayElementAtIndex (4), selectedVars.GetArrayElementAtIndex (4), true, true, 2);

			labelWithProperty ("Xbox Start", 140, varLocation.GetArrayElementAtIndex (5), selectedVars.GetArrayElementAtIndex (5), true, true, 2);

			labelWithProperty ("Xbox Right Stick", 140, varLocation.GetArrayElementAtIndex (6), selectedVars.GetArrayElementAtIndex (6), true, true, 2);

			labelWithProperty ("Xbox Right Shoulder", 140, varLocation.GetArrayElementAtIndex (7), selectedVars.GetArrayElementAtIndex (7), true, true, 2);

			labelWithProperty ("Xbox Left Stick", 140, varLocation.GetArrayElementAtIndex (8), selectedVars.GetArrayElementAtIndex (8), true, true, 2);

			labelWithProperty ("Xbox Left Shoulder", 140, varLocation.GetArrayElementAtIndex (9), selectedVars.GetArrayElementAtIndex (9), true, true, 2);

		} else {
			if (GUILayout.Button ("Display Misc Buttons")) {
				state.FindPropertyRelative ("displayMiscButtons").boolValue = true;
			}
		}


		EditorGUILayout.EndVertical ();
		GUILayout.Space (2);
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		if (state.FindPropertyRelative ("displayDpadButtons").boolValue) {
			if (GUILayout.Button ("Close Dpad Buttons")) {
				state.FindPropertyRelative ("displayDpadButtons").boolValue = false;
			}
			labelWithProperty ("Xbox Dpad Up", 140, varLocation.GetArrayElementAtIndex(10), selectedVars.GetArrayElementAtIndex (10), true, true, 2);

			labelWithProperty ("Xbox Dpad Down", 140, varLocation.GetArrayElementAtIndex(11), selectedVars.GetArrayElementAtIndex (11), true, true, 2);

			labelWithProperty ("Xbox Dpad Left", 140, varLocation.GetArrayElementAtIndex(12), selectedVars.GetArrayElementAtIndex (12), true, true, 2);
				
			labelWithProperty ("Xbox Dpad Right", 140, varLocation.GetArrayElementAtIndex(13), selectedVars.GetArrayElementAtIndex (13), true, true, 2);
		} else {
			if (GUILayout.Button ("Display Dpad Buttons")) {
				state.FindPropertyRelative ("displayDpadButtons").boolValue = true;
			}
		}

		EditorGUILayout.EndVertical ();
		GUILayout.Space (2);
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		if (state.FindPropertyRelative ("displayVariable").boolValue) {
			if (GUILayout.Button ("Close Changing Buttons")) {
				state.FindPropertyRelative ("displayVariable").boolValue = false;
			}
			labelWithProperty ("Xbox Left Stick Axis", 140, varLocation.GetArrayElementAtIndex (14), selectedVars.GetArrayElementAtIndex (14), true, true, 2);

			labelWithProperty ("Xbox Right Stick Axis", 140, varLocation.GetArrayElementAtIndex (15), selectedVars.GetArrayElementAtIndex (15), true, true, 2);

			labelWithProperty ("Xbox Trigger Left", 140, varLocation.GetArrayElementAtIndex (16), selectedVars.GetArrayElementAtIndex (16), true, true, 2);

			labelWithProperty ("Xbox Trigger Right", 140, varLocation.GetArrayElementAtIndex (17), selectedVars.GetArrayElementAtIndex (17), true, true, 2);

		} else {
			if (GUILayout.Button ("Display Changing Buttons")) {
				state.FindPropertyRelative ("displayVariable").boolValue = true;
			}
		}



		EditorGUILayout.EndVertical ();
	}

	void displayKeyData(SerializedProperty currentKey) {
		//display variable data block
		SerializedProperty data = currentKey.FindPropertyRelative ("data");
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		labelWithProperty ("Axis Value", 80, data.FindPropertyRelative ("axisValue"), true, true, 2);

		labelWithProperty ("Multi Axis Value", 100, data.FindPropertyRelative ("multiAxisValue"), true, true, 2);

		labelWithProperty ("Bool", 40, data.FindPropertyRelative ("b"), true, true, 2);

		labelWithProperty ("Byte", 40, data.FindPropertyRelative ("by"), true, true, 2);

		labelWithProperty ("Char", 40, data.FindPropertyRelative ("c"), true, true, 2);

		labelWithProperty ("Float", 40, data.FindPropertyRelative ("f"), true, true, 2);

		labelWithProperty ("Int", 30, data.FindPropertyRelative ("i"), true, true, 2);

		labelWithProperty ("GameObject", 80, data.FindPropertyRelative ("obj"), true, true, 2);

		labelWithProperty ("MonoBehaviour", 80, data.FindPropertyRelative ("sc"), true, true, 2);

		labelWithProperty ("String", 80, data.FindPropertyRelative ("s"), true, true, 2);

		labelWithProperty ("Transform", 80, data.FindPropertyRelative ("t"), true, true, 2);

		labelWithProperty ("Unity Event", 80, data.FindPropertyRelative ("u"), true, true, 2);

		labelWithProperty ("Vector2", 80, data.FindPropertyRelative ("v2"), true, true, 2);

		labelWithProperty ("Vector3", 80, data.FindPropertyRelative ("v3"), true, true, 2);

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Vector4", GUILayout.Width(80));
		data.FindPropertyRelative("v4").vector4Value = EditorGUILayout.Vector4Field("", data.FindPropertyRelative("v4").vector4Value);
		GUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical();
	}

	void labelWithProperty(string label, int width, SerializedProperty output, bool space = false, bool horizontal = false, int spaceSize = 0) {
		if (horizontal) {
			GUILayout.BeginHorizontal ();
		}
		if (space) {
			GUILayout.Space (spaceSize);
		}
		EditorGUILayout.LabelField (label, GUILayout.Width(width));
		EditorGUILayout.PropertyField(output, GUIContent.none);
		if (horizontal) {
			GUILayout.EndHorizontal ();
		}
	}

	void labelWithProperty(string label, int width, SerializedProperty output, SerializedProperty output2, bool space = false, bool horizontal = false, int spaceSize = 0) {
		if (horizontal) {
			GUILayout.BeginHorizontal ();
		}
		if (space) {
			GUILayout.Space (spaceSize);
		}
		EditorGUILayout.LabelField (label, GUILayout.Width(width));
		EditorGUILayout.PropertyField(output, GUIContent.none);
		EditorGUILayout.PropertyField (output2, GUIContent.none);
		if (horizontal) {
			GUILayout.EndHorizontal ();
		}
	}

}