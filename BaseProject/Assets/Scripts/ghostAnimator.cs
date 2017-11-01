using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostAnimator : MonoBehaviour {

	public ghostAnim animEnum;
	public float ignoreUpdate = 0;
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ignoreUpdate <= 0) {
			if (animEnum == ghostAnim.appear) {
				anim.Play ("Spawn", 0);
				animEnum = ghostAnim.idle;
				ignoreUpdate = PlayerController.getAnimationTime ("Spawn", anim);
			} else if (animEnum == ghostAnim.idle) {
				anim.Play ("Idle", 0);
				//ignoreUpdate = PlayerController.getAnimationTime ("idle", anim);
			} else if (animEnum == ghostAnim.buttonMash) {
				anim.Play ("Tickled", 0);
				animEnum = ghostAnim.idle;
				ignoreUpdate = PlayerController.getAnimationTime ("Tickled", anim);
			} else if (animEnum == ghostAnim.die) {
				anim.Play ("Death", 0);
				ignoreUpdate = PlayerController.getAnimationTime ("Death", anim);
				animEnum = ghostAnim.dieDisable;
			} else if (animEnum == ghostAnim.resurrect) {
				anim.Play ("Resurrect", 0);
				ignoreUpdate = PlayerController.getAnimationTime ("Resurrect", anim);
				animEnum = ghostAnim.disable;
			} else if (animEnum == ghostAnim.dieDisable) {
				Destroy (transform.parent.gameObject);
			} else {
				gameObject.SetActive (false);
				GetComponentInParent<PlayerController> ().resurrect ();
			}
		} else {
			ignoreUpdate -= Time.deltaTime;
		}
	}
}

public enum ghostAnim {
	appear,
	idle,
	die,
	resurrect,
	buttonMash,
	disable,
	dieDisable
}