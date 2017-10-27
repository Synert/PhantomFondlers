using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    public float attackTime = 200.0f;
    public bool angleLocked = true;
    float cooldown = 0.0f;

    public enum DIRECTION
    {
        UP,
        UP_RIGHT,
        RIGHT,
        DOWN_RIGHT,
        DOWN,
        DOWN_LEFT,
        LEFT,
        UP_LEFT
    }

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

    public bool GetAngleLock()
    {
        return angleLocked;
    }

    public float GetAttackTime()
    {
        return attackTime;
    }

    public DIRECTION GetDirection(int quad)
    {
        quad -= 2;
        if(quad < 0)
        {
            quad += 8;
        }
        return (DIRECTION)quad;
    }

    //attack a direction, for angle locked
    public virtual void AttackDirection(int quad, float power)
    {
        switch(GetDirection(quad))
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
    }

    //attack any angle, used mainly for ranged weapons
    public virtual void AttackAngle(float angle, float power)
    {

    }

    //charge an attack in direction
    public virtual void ChargeDirection(int quad, float power)
    {
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
    }

    //charge an attack at angle angle
    public virtual void ChargeAngle(float angle, float power)
    {

    }
}
