using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GameObject.FindObjectsOfType<DontDestroy>().Length != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
