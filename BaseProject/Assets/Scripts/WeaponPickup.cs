using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    public float attackTime = 200.0f;
    public bool angleLocked = true;
    public bool isRanged = false;
    float cooldown = 0.0f;
    public Transform projectile;
    public int damage;
    GameObject owner;
    bool held = false;
    public float powerCap;

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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChildUpdate();
    }

    public virtual void ChildUpdate()
    {
        if (cooldown > 0.0f)
        {
            cooldown -= 1000.0f * Time.deltaTime;
        }

        Vector3 temp = transform.localScale;
        if (transform.rotation.eulerAngles.z < 180)
        {
            //GetComponent<SpriteRenderer>().flipX = true;
            transform.localScale = new Vector3(-Mathf.Abs(temp.x), temp.y, temp.z);
        }
        else
        {
            //GetComponent<SpriteRenderer>().flipX = false;
            transform.localScale = new Vector3(Mathf.Abs(temp.x), temp.y, temp.z);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //on ground, attempt to pick up
        if (!held)
        {
            if (cooldown <= 0.0f)
            {
                if (col.tag == "Player")
                {
                    PlayerAttacks player = col.GetComponentInChildren<PlayerAttacks>();

                    if (GetOwner() == null)
                    {
                        player.SwitchWeapon(transform);

                    }
                }
            }
        }
        //held, so it's an attack
        else if(held && GetOwner() != null)
        {
            if (col.tag == "Player")
            {
                PlayerController player = col.GetComponentInChildren<PlayerController>();

                if (GetOwner().GetComponentInChildren<PlayerController>() != player)
                {
                    player.takeDamage(damage);
                    Debug.Log("aaa");
                }
            }
        }
    }

    public void Swapped()
    {
        cooldown = 1000.0f;
        /*if(owner != null)
        {
            owner = null;
            //held = false;
        }
        else
        {
            //held = true;
        }*/
    }

    public bool GetAngleLock()
    {
        return angleLocked;
    }

    public bool GetRanged()
    {
        return isRanged;
    }

    public float GetAttackTime()
    {
        return attackTime;
    }

    public DIRECTION GetDirection(int quad)
    {
        quad -= 2;
        if (quad < 0)
        {
            quad += 8;
        }
        return (DIRECTION)quad;
    }

    //attack a direction, for angle locked
    public virtual void AttackDirection(int quad, float power)
    {
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

    //attack any angle, used mainly for ranged weapons
    public virtual void AttackAngle(float angle, float power)
    {
        Quaternion orig = transform.rotation;
        transform.rotation = Quaternion.identity;
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * transform.right * 2.0f, Color.red, 1.0f);
        transform.rotation = orig;
    }

    //charge an attack in direction
    public virtual void ChargeDirection(int quad, float power)
    {
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

    //charge an attack at angle
    public virtual void ChargeAngle(float angle, float power)
    {
        Quaternion orig = transform.rotation;
        transform.rotation = Quaternion.identity;
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * transform.right * 2.0f, Color.green, 1.0f);
        transform.rotation = orig;
    }

    public void SetOwner(GameObject set)
    {
        owner = set;
        if (set == null)
        {
            held = false;
        }
        else
        {
            held = true;
        }
    }

    public GameObject GetOwner()
    {
        return owner;
    }
}