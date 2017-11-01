using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowCannon : WeaponPickup
{

    SpriteRenderer armSprite;
    SpriteRenderer pickupSprite;

    void Start()
    {
        foreach(Transform child in transform)
        {
            if(child.name == "ArmSprite")
            {
                armSprite = child.GetComponent<SpriteRenderer>();
            }
            if (child.name == "PickupSprite")
            {
                pickupSprite = child.GetComponent<SpriteRenderer>();
            }
        }
    }

    public override void ChildUpdate()
    {
        base.ChildUpdate();

        if(GetOwner() != null)
        {
            pickupSprite.enabled = false;
            armSprite.enabled = true;
        }
        else
        {
            pickupSprite.enabled = true;
            armSprite.enabled = false;
        }
    }

    public override void Attack(float power)
    {
        GetComponentInParent<BoxCollider2D>().enabled = false;
        Transform newProjectile = Instantiate(projectile, GetOwner().transform.position, transform.rotation);
        newProjectile.GetComponent<Projectile>().SetOwner(GetOwner());
        newProjectile.GetComponent<Projectile>().SetDamage(damage);

        DestroyImmediate(gameObject);
    }

    public override void Charge(float power)
    {
        //
    }

}
