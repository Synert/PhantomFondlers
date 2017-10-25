using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : MonoBehaviour
{
    public KeyCode InvincibilityKey;
    public float SpiritLevel = 1.0f;
    private float currCountdownValue = 0.1f;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown (InvincibilityKey))
        {
            SpiritLevel -= 0.75f;
            StartCoroutine(StartCountdown());
        }
        if(currCountdownValue == 0)
        {
            SpiritLevel += 0.75f;
        }
        if(SpiritLevel > 1)
        {
            SpiritLevel = 1;
        }

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, SpiritLevel);
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
