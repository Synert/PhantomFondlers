using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour {

    
    
    public GameObject obj;
    // Use this for initialization
    void Start () {
       
        obj = GameObject.Find("PlayerVal").gameObject;
     
    }
	
    public void GetInputHealth(string healthstuff)
    {
        int temp = int.Parse(healthstuff);
        obj.GetComponent<PlayerValues>().health = temp;
    }
    public void GetRespawnHealth(string respawnHealthStuff)
    {
        int temp2 = int.Parse(respawnHealthStuff);
        obj.GetComponent<PlayerValues>().respawnHealth = temp2;
    }
    public void GetSpamCount(string spamNum)
    {
        int temp3 = int.Parse(spamNum);
        obj.GetComponent<PlayerValues>().spamCount = temp3;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
