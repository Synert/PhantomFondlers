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

    public override void AttackDirection(int quad, float power)
    {
        attackAnimTime = 400.0f;
        //GetComponentInChildren<ParticleSystem>().Clear();
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().enableEmission = false;

        GetComponentInChildren<Animator>().Play("FeatherDuster", 0);

        GetOwner().GetComponent<Rigidbody2D>().AddForce(transform.up * power * 125.0f);

        Debug.Log("Attack power " + power);
        switch (GetDirection(quad))
        {
            case DIRECTION.UP:
                Debug.Log("Up attack");
                break;
            case DIRECTION.UP_RIGHT:
                //
                break;
            case DIRECTION.RIGHT:
                Debug.Log("Right attack");
                break;
            case DIRECTION.DOWN_RIGHT:
                //
                break;
            case DIRECTION.DOWN:
                Debug.Log("Down attack");
                break;
            case DIRECTION.DOWN_LEFT:
                //
                break;
            case DIRECTION.LEFT:
                Debug.Log("Left attack");
                break;
            case DIRECTION.UP_LEFT:
                //
                break;
        }
    }

    public override void ChargeDirection(int quad, float power)
    {
        GetComponentInChildren<ParticleSystem>().enableEmission = true;
        if(!GetComponentInChildren<ParticleSystem>().isPlaying)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
        Quaternion orig = transform.rotation;
        transform.rotation = Quaternion.identity;
        switch (GetDirection(quad))
        {
            case DIRECTION.UP:
                //
                break;
            case DIRECTION.UP_RIGHT:
                //
                break;
            case DIRECTION.RIGHT:
                //
                break;
            case DIRECTION.DOWN_RIGHT:
                //
                break;
            case DIRECTION.DOWN:
                //
                break;
            case DIRECTION.DOWN_LEFT:
                //
                break;
            case DIRECTION.LEFT:
                //
                break;
            case DIRECTION.UP_LEFT:
                //
                break;
        }
        transform.rotation = orig;
    }
}
