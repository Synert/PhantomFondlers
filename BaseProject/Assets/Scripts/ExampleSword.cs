using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSword : WeaponPickup {

    public override void AttackDirection(int quad, float power)
    {
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

    public override void AttackAngle(float angle, float power)
    {
        Quaternion orig = transform.rotation;
        transform.rotation = Quaternion.identity;
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * transform.right * 2.0f, Color.red, 1.0f);
        transform.rotation = orig;
    }

    public override void ChargeDirection(int quad, float power)
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

    public override void ChargeAngle(float angle, float power)
    {
        Quaternion orig = transform.rotation;
        transform.rotation = Quaternion.identity;
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(angle, Vector3.forward) * transform.right * 2.0f, Color.green, 1.0f);
        transform.rotation = orig;
    }

}
