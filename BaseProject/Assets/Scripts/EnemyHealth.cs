using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    void Start()
    {

        curHealth = maxHealth;

    }
    void Update()
    {
        if (curHealth < 1)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bullet")
        {
            curHealth -= 1;
            Destroy(col.gameObject);
        }
    }
}
