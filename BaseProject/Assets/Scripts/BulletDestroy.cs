using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroy : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            DestroyObject(other.gameObject);
            DestroyObject(gameObject);
        }
        else
        {
            DestroyObject(gameObject);
        }
    }
}
