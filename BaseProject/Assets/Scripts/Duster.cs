using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duster : WeaponPickup {

    float attackAnimTime = 0.0f;

    public override void ChildUpdate()
    {
        base.ChildUpdate();

        if(attackAnimTime > 0.0f)
        {
            GetComponentsInChildren<BoxCollider2D>()[1].enabled = true;

            attackAnimTime -= 1000.0f * Time.deltaTime;
        }
        else
        {
           GetComponentsInChildren<BoxCollider2D>()[1].enabled = false;
            GetComponentInChildren<Animator>().Play("FeatherDusterIdle", 0);
        }
    }

    public override void Attack(float power)
    {
        base.Attack(power);

        GetComponentsInChildren<BoxCollider2D>()[1].enabled = true;
        attackAnimTime = 400.0f;
        //GetComponentInChildren<ParticleSystem>().Clear();
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().enableEmission = false;
        GetComponentInChildren<Animator>().Play("FeatherDuster", 0);
        GetOwner().GetComponent<Rigidbody2D>().AddForce(transform.up * power * 125.0f);
        Debug.Log("Attack power " + power);
    }

    public override void Charge(float power)
    {
        base.Charge(power);

        GetComponentInChildren<ParticleSystem>().enableEmission = true;
        if(!GetComponentInChildren<ParticleSystem>().isPlaying)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }
}
