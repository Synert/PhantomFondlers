using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSword : WeaponPickup {

    public override void Attack(float power)
    {
        Transform newProjectile = Instantiate(projectile, GetOwner().transform.position, transform.rotation);
        newProjectile.GetComponent<Projectile>().SetOwner(GetOwner());
        newProjectile.GetComponent<Projectile>().SetDamage(damage);
    }

    public override void Charge(float power)
    {
        //
    }

}