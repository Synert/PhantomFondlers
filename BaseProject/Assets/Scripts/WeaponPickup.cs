using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    float cooldown = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (cooldown > 0.0f)
        {
            cooldown -= 1000.0f * Time.deltaTime;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (cooldown <= 0.0f)
        {
            if (col.tag == "Player")
            {
                PlayerAttacks player = col.GetComponentInChildren<PlayerAttacks>();

                player.SwitchWeapon(transform);
            }
        }
    }

    public void Swapped()
    {
        cooldown = 1000.0f;
    }
}
