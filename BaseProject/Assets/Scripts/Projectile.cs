using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    GameObject owner;
    public float speed;
    int damage = 0;
    int bounceCount = 6;
    float lifeSpan = 8000.0f;

	// Use this for initialization
	void Start () {
        //transform.Translate(0, 0, -5);
	}
	
	// Update is called once per frame
	void Update () {
        //GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        lifeSpan -= 1000.0f * Time.deltaTime;
        if(lifeSpan <= 0.0f)
        {
            Destroy(gameObject);
        }
	}

    public void SetOwner(GameObject set)
    {
        owner = set;
        GetComponent<Rigidbody2D>().AddForce(700.0f * transform.up);
    }

    public void SetDamage(int set)
    {
        damage = set;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(owner == null)
        {
            Debug.Log("aaaa");
            return;
        }
        if(col.gameObject != owner && col.gameObject.tag != "Weapon")
        {
            PlayerController player = col.gameObject.GetComponentInChildren<PlayerController>();
            if(player != null)
            {
                //replace with damage function later
                Debug.Log("slap");
                player.takeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
