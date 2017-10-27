using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;

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
			if (visible.GetArrayElementAtIndex (a).boolValue) {
				//grab current keyInfo
				SerializedProperty currentKeySet = keys.GetArrayElementAtIndex (a);

				//grab the current keysets
				SerializedProperty currentKeyButtonList = currentKeySet.FindPropertyRelative ("buttons");
				SerializedProperty currentKeyAxisList = currentKeySet.FindPropertyRelative ("axis");

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
					
				GUILayout.BeginVertical (EditorStyles.helpBox);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				GUILayout.FlexibleSpace ();
				//display adding for multiple axxis
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
					EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("multiAxis"));
					EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axis"));
					if (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("multiAxis").boolValue) {
						EditorGUILayout.PropertyField (currentKeyAxisList.GetArrayElementAtIndex (b).FindPropertyRelative ("axis2"));
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
						EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionObject"));
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
					//grab data
					EditorGUILayout.PropertyField (currentKeyButtonList.GetArrayElementAtIndex (b).FindPropertyRelative ("button"));
					EditorGUILayout.PropertyField (currentKeyButtonList.GetArrayElementAtIndex (b).FindPropertyRelative ("keyType"));
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
						EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("functionObject"));
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
		}
			
		GUILayout.EndVertical();
		
		serializedObject.ApplyModifiedProperties ();
	}

	void displayKeyData(SerializedProperty currentKey) {
		//display variable data block
		SerializedProperty data = currentKey.FindPropertyRelative ("data");
		EditorGUILayout.BeginVertical (EditorStyles.helpBox);

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Axis Value", GUILayout.Width(80));
		EditorGUILayout.FloatField (data.FindPropertyRelative ("axisValue").floatValue);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Multi Axis Value", GUILayout.Width(100));
		EditorGUILayout.Vector2Field ("", data.FindPropertyRelative ("multiAxisValue").vector2Value);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Bool", GUILayout.Width(40));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("b"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Byte", GUILayout.Width(40));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("by"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Char", GUILayout.Width(40));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("c"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Float", GUILayout.Width(40));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("f"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Int", GUILayout.Width(40));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("i"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("GameObject", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("obj"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("MonoBehaviour", GUILayout.Width(100));
		EditorGUILayout.ObjectField(data.FindPropertyRelative("sc"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("String", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("s"), GUIContent.none);
		GUILayout.EndHorizontal ();


		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Transform", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("t"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Unity Event", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("u"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Vector2", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("v2"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Vector3", GUILayout.Width(80));
		EditorGUILayout.PropertyField(data.FindPropertyRelative("v3"), GUIContent.none);
		GUILayout.EndHorizontal ();

		//split up the data
		GUILayout.Space (2);
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Vector4", GUILayout.Width(80));
		data.FindPropertyRelative("v4").vector4Value = EditorGUILayout.Vector4Field("", data.FindPropertyRelative("v4").vector4Value);
		GUILayout.EndHorizontal ();

		EditorGUILayout.EndVertical();
	}

}