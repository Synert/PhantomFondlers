using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowCannon : WeaponPickup
{

    public override void Attack(float power)
    {
        Transform newProjectile = Instantiate(projectile, GetOwner().transform.position, transform.rotation);
        newProjectile.GetComponent<Projectile>().SetOwner(GetOwner());
        newProjectile.GetComponent<Projectile>().SetWeapon(gameObject);
        newProjectile.GetComponent<Projectile>().SetDamage(damage);

        Destroy(gameObject);
    }

    public override void Charge(float power)
    {
        //
    }

}
