﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincibility : MonoBehaviour
{
    public PlayerController playerController;
    public KeyCode InvincibilityKey;
    public bool isInvincible;
    public float SpiritLevel = 1.0f;
    public bool isDogding = false;
    Animator anim;

    private float currCountdownValue = 0.1f;
    private float direction;
    private Vector2 destitation;
    private float cooldown = 0f;
    private float duration = 0f;
    float originalDistance;
	bool intoWall = false;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
		playerController = GetComponent<PlayerController> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (currCountdownValue == 0)
        {
            SpiritLevel += 0.75f;
        }
        if (SpiritLevel > 1)
        {
            SpiritLevel = 1;
        }
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, SpiritLevel);

        if (isDogding)
        {
            movement(direction);

        }
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
            
    
    }

	public bool pollIsDodging() {
		return isDogding;
	}

    void dodge(variableData _var)
    {
        float stickVel = -_var.state.ThumbStickLeft.inputs.x;
		if (playerController.isOnGround && !(playerController.stat == status.death || playerController.stat == status.deathAnim))
        {
            if (cooldown <= 0)
            {
                if (GetComponent<Rigidbody2D>().velocity.x > -playerController.MaxSpeed && stickVel < 0)
                {
					intoWall = false;
					playerController.playSound(4, 1, 6);
                    //anim.SetInteger("State", 4);
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(-playerController.Acceleration, 0.0f));
                    Debug.Log("Moving Right");
                    direction = 1;
                    cooldown = 2.0f;
                    duration = 1.5f;
                    destitation = new Vector2((transform.position.x + 10), transform.position.y);
                    originalDistance = Vector2.Distance(transform.position, destitation);
                    playerController.movementPause = true;
                    isDogding = true;
                    isInvincible = true;

                }
                else if (GetComponent<Rigidbody2D>().velocity.x < playerController.MaxSpeed && stickVel > 0)
				{
					intoWall = false;
					playerController.playSound(4, 1, 1);
                    //anim.SetInteger("State", 4);
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(playerController.Acceleration, 0.0f));
                    Debug.Log("Moving Left");
                    direction = -1;
                    cooldown = 2.0f;
                    duration = 1.5f;
                    destitation = new Vector2((transform.position.x - 10), transform.position.y);
                    originalDistance = Vector2.Distance(transform.position, destitation);
                    playerController.movementPause = true;
                    isDogding = true;
                    isInvincible = true;
                }
            }

        }
        //StartCoroutine(StartCountdown());
    }

    void movement(float direction)
    {
		if (direction == 1) {
			if (playerController.rightWall) {
				intoWall = true;

			}
		} else if (direction == -1) {
			if (playerController.leftWall) {
				intoWall = true;
			}
		}
		if (playerController.stat == status.death || playerController.stat == status.deathAnim) {
			isDogding = false;
			isInvincible = false;
			playerController.movementPause = false;
		}
        duration -= Time.deltaTime;
        float currentDistance = Vector2.Distance(transform.position, destitation);
		if (duration <= 1f && !intoWall)
        { 
            transform.position = Vector2.Lerp(transform.position, destitation, Time.deltaTime + ((currentDistance / originalDistance) * Time.deltaTime));
        }
        if(duration <= 0)
        {
            isDogding = false;
            isInvincible = false;
            playerController.movementPause = false;
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
