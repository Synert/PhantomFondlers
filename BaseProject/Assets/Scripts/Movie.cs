using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]

public class Movie : MonoBehaviour {
    public MovieTexture movie;
	// Use this for initialization
	void Start () {

        GetComponent<RawImage>().texture = movie as MovieTexture;
	}
	

}
