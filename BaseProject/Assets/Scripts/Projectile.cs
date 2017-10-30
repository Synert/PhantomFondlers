using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    GameObject owner;
    GameObject weapon;
    int damage = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody2D>().velocity = transform.up * 5.0f;
	}

    public void SetOwner(GameObject set)
    {
        owner = set;
    }

    public void SetWeapon(GameObject set)
    {
        weapon = set;
    }

    public void SetDamage(int set)
    {
        damage = set;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(owner == null || weapon == null)
        {
            Debug.Log("aaaa");
            return;
        }
        if(col.gameObject != owner && col.gameObject != weapon)
        {
            PlayerController player = col.gameObject.GetComponentInChildren<PlayerController>();
            if(player != null)
            {
                //replace with damage function later
                player.takeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
