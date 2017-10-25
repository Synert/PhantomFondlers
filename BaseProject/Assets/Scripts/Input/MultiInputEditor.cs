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
			if (visible.GetArrayElementAtIndex (a).boolValue) {
				SerializedProperty currentKey = keys.GetArrayElementAtIndex (a);
				SerializedProperty currentKeyList = currentKey.FindPropertyRelative ("key");
				SerializedProperty axisInput = keys.GetArrayElementAtIndex (a).FindPropertyRelative ("axisInput");
				SerializedProperty privateFunction = keys.GetArrayElementAtIndex (a).FindPropertyRelative ("privateFunction");
				SerializedProperty offset = keys.GetArrayElementAtIndex (a).FindPropertyRelative ("offset");
				SerializedProperty keyType = keys.GetArrayElementAtIndex (a).FindPropertyRelative ("keyType");

				//allow closing of element data
				GUI.backgroundColor = new Color (1, 1, 1, 1);
				GUILayout.BeginHorizontal (EditorStyles.helpBox);
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Close Element " + a.ToString (), GUILayout.Width (300))) {
					visible.GetArrayElementAtIndex (a).boolValue = false;
				}
				GUILayout.FlexibleSpace ();
				GUI.backgroundColor = new Color (0.9f, 0.9f, 0.9f, 0.5f);
				GUILayout.EndHorizontal ();

				//start data section
				GUILayout.BeginVertical (EditorStyles.helpBox);

				GUILayout.BeginHorizontal (EditorStyles.helpBox);

				GUILayout.FlexibleSpace ();
				//display adding for multiple keys
				if (GUILayout.Button ("Add Key", GUILayout.Width (150))) {
					//add new blank key data
					currentKeyList.InsertArrayElementAtIndex (currentKeyList.arraySize);
					offset.InsertArrayElementAtIndex (offset.arraySize);
					keyType.InsertArrayElementAtIndex (keyType.arraySize);
				}
				if (GUILayout.Button ("Remove Key", GUILayout.Width (150))) {
					//check if key data has data
					if (currentKeyList.arraySize > 0) {
						//remove last key data
						currentKeyList.DeleteArrayElementAtIndex (currentKeyList.arraySize - 1);
						offset.DeleteArrayElementAtIndex (offset.arraySize - 1);
						keyType.DeleteArrayElementAtIndex (keyType.arraySize - 1);
					}
				}
				GUILayout.FlexibleSpace ();

				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);

				GUILayout.BeginHorizontal ();

				//display bool for variable input
				EditorGUILayout.LabelField ("Variable Input", GUILayout.Width (100));
				keys.GetArrayElementAtIndex (a).FindPropertyRelative ("variableInput").boolValue
					= EditorGUILayout.Toggle (keys.GetArrayElementAtIndex (a).FindPropertyRelative
												("variableInput").boolValue, GUILayout.Width (20));
				
				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);
				GUILayout.BeginHorizontal ();

				//display boo for private function
				EditorGUILayout.LabelField ("Private Function", GUILayout.Width (100));
				privateFunction.boolValue = EditorGUILayout.Toggle (privateFunction.boolValue, GUILayout.Width (20));

				if (privateFunction.boolValue) {
					EditorGUILayout.PropertyField (keys.GetArrayElementAtIndex (a).FindPropertyRelative ("privateFunctionName"), GUIContent.none, GUILayout.Width (190));

					//split up the options
					GUILayout.EndHorizontal ();
					GUILayout.Space (2);
					GUILayout.BeginHorizontal ();

					EditorGUILayout.LabelField ("GameObject", GUILayout.Width (125));
					EditorGUILayout.ObjectField (keys.GetArrayElementAtIndex (a).FindPropertyRelative ("functionObject"), GUIContent.none, GUILayout.Width (190));
				}

				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);
				GUILayout.BeginHorizontal ();

				//display bool for axis input
				EditorGUILayout.LabelField ("Axis Input", GUILayout.Width (100));
				axisInput.boolValue = EditorGUILayout.Toggle (axisInput.boolValue, GUILayout.Width (20));

				if (axisInput.boolValue) {
					EditorGUILayout.PropertyField (keys.GetArrayElementAtIndex (a).FindPropertyRelative ("axis"), GUIContent.none, GUILayout.Width (190));
					keys.GetArrayElementAtIndex (a).FindPropertyRelative ("variableInput").boolValue = true;
					privateFunction.boolValue = true;
				}


				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);

				GUILayout.BeginHorizontal ();

				//display int field for max accepted keys per update
				EditorGUILayout.LabelField ("Max Inputs Per Update", GUILayout.Width (135));
				keys.GetArrayElementAtIndex (a).FindPropertyRelative ("keyInputsAccepted").intValue = 
					EditorGUILayout.IntField (keys.GetArrayElementAtIndex (a).FindPropertyRelative ("keyInputsAccepted").intValue, GUILayout.Width (180));

				//split up the options
				GUILayout.EndHorizontal ();
				GUILayout.Space (2);

				//apply update to data
				serializedObject.ApplyModifiedProperties ();

				for (int b = 0; b < currentKeyList.arraySize; b++) {

					GUILayout.BeginHorizontal ();

					//display popup for key input
					EditorGUILayout.LabelField ("Key", GUILayout.Width (40));
					EditorGUILayout.PropertyField (currentKeyList.GetArrayElementAtIndex (b), GUIContent.none, GUILayout.Width (275));
					EditorGUILayout.PropertyField (offset.GetArrayElementAtIndex (b), GUIContent.none, GUILayout.Width (40));
					EditorGUILayout.PropertyField (keyType.GetArrayElementAtIndex (b), GUIContent.none, GUILayout.Width (100));


					//split up the options
					GUILayout.EndHorizontal ();
					GUILayout.Space (2);

				}

				if (!privateFunction.boolValue) {
					GUILayout.BeginHorizontal ();

					//display string for key event
					EditorGUILayout.LabelField ("Event", GUILayout.Width (40));
					EditorGUILayout.PropertyField (currentKey.FindPropertyRelative ("keyEvent"), GUILayout.Width (415));


					GUILayout.EndHorizontal ();
				}

				//end data section
				GUILayout.EndVertical ();
				GUILayout.Space (10);

				serializedObject.ApplyModifiedProperties ();

			} else {
				
				//allow opening of element
				GUI.backgroundColor = new Color(1,1,1,1);
				GUILayout.BeginHorizontal(EditorStyles.helpBox);
				GUILayout.Space (100);
				if (GUILayout.Button ("Show Element " + a.ToString (), GUILayout.Width(300))) {
					visible.GetArrayElementAtIndex (a).boolValue = true;
				}
				GUI.backgroundColor = new Color(0.9f,0.9f,0.9f,0.5f);
				GUILayout.EndHorizontal();
				GUILayout.Space (10);
			}
		}
			
		GUILayout.EndVertical();
		
		serializedObject.ApplyModifiedProperties ();
	}

}