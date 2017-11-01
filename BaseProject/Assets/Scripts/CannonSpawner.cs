using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSpawner : MonoBehaviour {

    public Transform[] spawnWeapon;
    Transform spawnedWeapon;
    float spawnCooldown = 10.0f;
    float cooldown = 0.0f;
    bool startCooldown = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(cooldown > 0.0f && startCooldown)
        {
            cooldown -= Time.deltaTime;
        }
		if(spawnedWeapon == null)
        {
            if(startCooldown && cooldown <= 0.0f)
            {
                startCooldown = false;
                spawnedWeapon = Instantiate(spawnWeapon[Random.Range(0, spawnWeapon.Length)], transform.position, Quaternion.identity);
                cooldown = spawnCooldown;
            }
            else if(!startCooldown)
            {
                startCooldown = true;
            }
        }
	}
}
