using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioSource m_audio;
    public AudioClip[] m_songs = new AudioClip[2];

	// Use this for initialization
	void Start () {
        m_audio.clip = m_songs[Random.Range(0, m_songs.Length)];
        m_audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
