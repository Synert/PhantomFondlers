using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    private bool CanTakeDamage;

    public KeyCode InvincibilityKey;
    public float SpiritLevel = 1.0f;
    private float currCountdownValue = 0.1f;
    private bool IsInvisible;

    void Start()
    {
        curHealth = maxHealth;
        CanTakeDamage = true;
        IsInvisible = false;
    }

    void Update()
    {
        if (curHealth < 1)
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(InvincibilityKey))
        {
            IsInvisible = true;
            StartCoroutine(StartCountdown());
        }
        if (IsInvisible == true)
        {
            SpiritLevel = 0.25f;
        }
        if (currCountdownValue == 0)
        {
            CanTakeDamage = true;
            IsInvisible = false;
            SpiritLevel = 1.0f;
        }
        
        if (IsInvisible == true)
        {
            CanTakeDamage = false;
        }

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, SpiritLevel);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "Bullet" && CanTakeDamage == true)
        {
            curHealth -= 1;
            Destroy(col.gameObject);
        }
    }

    public IEnumerator StartCountdown(float countdownValue = 10)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            Debug.Log("Countdown: " + currCountdownValue);
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
    }
}
