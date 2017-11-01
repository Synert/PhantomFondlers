using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xButtonAnim : MonoBehaviour {

	public buttonAnim animEnum;
	public float ignoreUpdate = 0;
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ignoreUpdate <= 0) {
			if (animEnum == buttonAnim.pressed) {
				anim.Play ("XButton", 0);
				ignoreUpdate = PlayerController.getAnimationTime ("XButton", anim);
				animEnum = buttonAnim.idle;
			} else {
				anim.Play ("XButtonIdle", 0);
			}
		} else {
			ignoreUpdate -= Time.deltaTime;
		}
	}
}

public enum buttonAnim {
	idle,
	pressed
}