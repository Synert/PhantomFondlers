using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Invincibility invincibility;
    public PlayerAttacks m_attack;
    public Renderer rend = new Renderer();
    public float MaxSpeed = 6;
    public float Acceleration = 5;
    public float jumpSpeed = 5;
    public float jumpDuration;
	public bool isOnGround = false;
	public float slowDownSpeed;

    //===== Data for Animation ====//
    Animator anim;
    private bool facingRight;
    public Vector2 originalSize;
    //=============================

    public bool rotateAroundObject = true;
	public float rotationSpeed = 1;
	public float resetJumpHeight = 1;
	public Vector2 MinMaxZRot = new Vector2();


    public bool leftWall;
    public bool rightWall;

    public bool enableWallJump = true;
    public bool enableDoubleJump = true;
    public bool wallHitJump = true;
    public bool leftWallHit = false;
    public bool rightWallHit = false;
    public bool Jumping = false;
    //===== Data for buttonMashing ====//
    public float arbitaryHealth = 0;
	public PlayerController ontopOf;
	public float increaseCount = 0;
	public float increaseWhenOntopCount = 0;
	public float currentCount = 0;
	public float respawnCount = 100;
	public float respawnHealth = 10;
    //=================================
    //====== Data for sprinting ======//
    public float sprintSpeedModifier;
    //=================================
	//temp
	public status stat;
	public float animNeedsFinish = 0;
	//

    bool canDoubleJump = true;
    float jmpDuration;

    bool keyPressDown = false;
    bool canJumpVariable = false; 

	bool horizontal = false;
	public bool movementPause = false;

    //Audio
    public AudioClip[] AudioClip;
   
    
    void Start()
    {
        facingRight = true;
        anim = GetComponent<Animator>();
        rend = GetComponent<Renderer>();

        originalSize = GetComponent<Collider2D>().bounds.size;
        Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
        GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
    }

	//==============================================
	//Deal with buttonMashing to revive/tickle to death
	//needs to be polished but fundementals are done
	public void takeDamage(int damage) {
		if (!movementPause) {
            arbitaryHealth -= damage;
			if (arbitaryHealth <= 0)
            {
                stat = status.death;
                anim.Play("death", 0);
                animNeedsFinish = getAnimationTime("death");
                //GetComponent<BoxCollider2D>().isTrigger = true;
                GetComponent<Rigidbody2D>().mass = 100;
                movementPause = true;
                Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
                GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
                playSound(3);
			} else
            {
                playSound(2);
            }
		}
	}

	//if health below 0 count up
	//if person currently ontop count up slower
	//if health above 0 and above another
	//if other health below 0 count down other
	void buttonMash () {
		if (arbitaryHealth <= 0) {
			GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			if (ontopOf) {
				currentCount += increaseWhenOntopCount;
			} else {
				currentCount += increaseCount;
			}
			testHealth ();
		} else {
			if (ontopOf) {
				if (ontopOf.arbitaryHealth <= 0) {
					GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
					movementPause = true;
					ontopOf.currentCount -= increaseCount;
					ontopOf.testHealth ();
				}
			}
		}
	}

	//check if arbitary health is 
	//below 0 or if currenCount is
	//above respawn and take actions
	public void testHealth() {
		if (currentCount <= -1) {
			ontopOf.movementPause = false;
			ontopOf.ontopOf = null;
			Destroy (this.gameObject);
		} else if (currentCount >= respawnCount)
        {
            //GetComponent<BoxCollider2D>().isTrigger = false;
            GetComponent<Rigidbody2D>().mass = 1;
            movementPause = false;
			arbitaryHealth = respawnHealth;
			currentCount = 0;
		}
	}

	//if other has no ontop set ontop of other and self to required
	void OnCollisionEnter2D (Collision2D _other) {
        Collider2D other = _other.collider;
		if (other.GetComponent<PlayerController> ()) {
			if (other.GetComponent<PlayerController> ().arbitaryHealth <= 0) {
				if (!movementPause) {
					if (!other.GetComponent<PlayerController> ().ontopOf) {
						ontopOf = other.GetComponent<PlayerController> ();
						other.GetComponent<PlayerController> ().ontopOf = this;
					}
				}
			}
		}
	}

	//if other has ontop set ontop of other and self to required
	void OnCollisionExit2D(Collision2D _other)
    {
        Collider2D other = _other.collider;
        if (other.GetComponent<PlayerController> ()) {
			if (ontopOf == this) {
				if (other.GetComponent<PlayerController> ().ontopOf) {
					ontopOf = null;
					other.GetComponent<PlayerController> ().ontopOf = null;
				}
			}
		}
	}
    //====================================================

    void inputs(variableData variable)
    {
		float tempAxis = variable.state.ThumbStickLeft.inputs.x;
        if (!movementPause)
        {
            if (Mathf.Abs(tempAxis) < 0.9f)
            {
                if (tempAxis < 0)
                {
                    if (GetComponent<Rigidbody2D>().velocity.x > -this.MaxSpeed)
                    {
                        if (isOnGround)
						{
							anim.StopPlayback ();
							//anim.SetInteger("State", 2);
							anim.Play ("WalkingARMS" , 0);
							stat = status.walking;
                            
                        }
                        GetComponent<SpriteRenderer>().flipX = true;
                        facingRight = false;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-this.Acceleration, 0.0f));
                    }
                }
                else
                {
                    if (GetComponent<Rigidbody2D>().velocity.x < this.MaxSpeed)
                    {
                        if (isOnGround)
						{
							anim.StopPlayback ();
							//anim.SetInteger("State", 2);
							anim.Play ("WalkingARMS" , 0);
							stat = status.walking;
                            
                        }
                        GetComponent<SpriteRenderer>().flipX = false;
                        facingRight = true;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(this.Acceleration, 0.0f));
                    }
                }
            }
            else
            {
                if (tempAxis < 0)
                {
                    if (GetComponent<Rigidbody2D>().velocity.x > -this.MaxSpeed * sprintSpeedModifier)
                    {
                        if (isOnGround)
						{
							anim.StopPlayback ();
							//anim.SetInteger("State", 2);
							anim.Play ("RunningARMS" , 0);
							stat = status.running;
                            
                        }
                        GetComponent<SpriteRenderer>().flipX = true;
                        facingRight = false;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(-this.Acceleration * sprintSpeedModifier, 0.0f));
                    }
                }
                else
                {
                    if (GetComponent<Rigidbody2D>().velocity.x < this.MaxSpeed * sprintSpeedModifier)
                    {
                        if (isOnGround)
						{
							anim.StopPlayback ();
							//anim.SetInteger("State", 2);
							anim.Play ("RunningARMS" , 0);
							stat = status.running;
                            
                        }
                        GetComponent<SpriteRenderer>().flipX = false;
                        facingRight = true;
                        GetComponent<Rigidbody2D>().AddForce(new Vector2(this.Acceleration * sprintSpeedModifier, 0.0f));
                    }
                }
            }
            horizontal = true;
        }
    }

    //   void jump() {
    //	if (!movementPause) {
    //		if (!keyPressDown) {
    //			keyPressDown = true;
    //			if (isOnGround || (canDoubleJump && enableDoubleJump || wallHitJump && enableWallJump)) {
    //				bool wallHit = false;
    //				int wallHitDirection = 0;

    //				leftWallHit = onLeftWall ();
    //				rightWallHit = onRightWall ();

    //                   if (isOnGround || (canDoubleJump && enableDoubleJump) || leftWallHit || rightWallHit)
    //                   {
    //                       if (isOnGround == true)
    //                       {
    //                           //Jumping from ground
    //                           playSound(0, 1.25f);
    //                           anim.SetInteger("State", 3);
    //                       }
    //                       else if (!isOnGround && !(leftWallHit || rightWallHit))
    //                       {
    //                           //Double jump
    //                           playSound(0, 0.5f);
    //                           anim.SetInteger("State", 3);
    //                       }
    //                       else
    //                       {
    //                           Debug.Log("JUMPING FROM WALL");
    //                           //Jumping from wall
    //                           JumpingFromWall = true;
    //                           anim.SetInteger("State", 6);
    //                           playSound(0, 0.7f);
    //                       }
    //                   }

    //                   if (horizontal) {
    //					if (leftWallHit && enableWallJump) {
    //                           wallHit = true;
    //						wallHitDirection = 1;
    //					} else if (rightWallHit && enableWallJump) {

    //                           wallHit = true;
    //						wallHitDirection = -1;
    //					}
    //				}

    //				if (!wallHit) {
    //					if (isOnGround || (canDoubleJump && enableDoubleJump)) {
    //                           GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
    //						jumpDuration = 0.0f;
    //						canDoubleJump = true;
    //					}
    //				} else {

    //					GetComponent<Rigidbody2D> ().velocity = new Vector2 (this.jumpSpeed * wallHitDirection, this.jumpSpeed);
    //                       jmpDuration = 0.0f;
    //					canJumpVariable = true;
    //				}

    //				if (!isOnGround && !wallHit) {
    //					canDoubleJump = false;
    //				}
    //			}
    //		} else if (canJumpVariable) {
    //			jmpDuration += Time.deltaTime;

    //			if (jmpDuration < this.jumpDuration / 1000) {
    //				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
    //			}
    //		}
    //	}
    //}


    // Update is called once per frame

	void jump() {
		if (!movementPause) {
			if (!keyPressDown) {

				keyPressDown = true;
				if (isOnGround || (canDoubleJump && enableDoubleJump) || (wallHitJump && enableWallJump)) {
					bool wallHit = false;
					int wallHitDirection = 0;

					if (leftWall || rightWall) {
						wallHit = true;
					}

					if (horizontal) {
						if (wallHit) {
							if (enableWallJump) {
								if (leftWall) {
									wallHitDirection = 1;
								} else if (rightWall) {
									wallHitDirection = -1;
								}
							}
						}
					}

					if (!wallHit) {
						if (isOnGround || (canDoubleJump && enableDoubleJump)) {
							//anim.SetInteger ("State", 3);
							anim.Play ("StandingJump" , 0);
							animNeedsFinish = getAnimationTime("StandingJump");
							if (!isOnGround) {
								canDoubleJump = false;
								playSound (0, 0.5f);
							} else {
								playSound (0, 1.25f);
								canDoubleJump = true;
							}
							stat = status.standingJump;
							GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
							jumpDuration = 0.0f;
						}
					} else {
						if (isOnGround) {
							//anim.SetInteger ("State", 3);
							anim.Play ("StandingJump" , 0);
							animNeedsFinish = getAnimationTime("StandingJump");
							playSound (0, 1.25f);
							stat = status.standingJump;
							GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
							jmpDuration = 0.0f;
							canJumpVariable = true;
						} else {
							if (wallHitDirection != 0) {
								//do wall jump here
								playSound (0, 0.7f);
								//anim.SetInteger ("State", 6);
								anim.Play ("WallJump" , 0);
								animNeedsFinish = getAnimationTime("WallJump") * 2;
								stat = status.wallJump;
								GetComponent<Rigidbody2D> ().velocity = new Vector2 (this.jumpSpeed * wallHitDirection, this.jumpSpeed);
								jmpDuration = 0.0f;
								canJumpVariable = true;
							}
						}


					}


				}
			} else if (canJumpVariable) {
				jmpDuration += Time.deltaTime;

				if (jmpDuration < this.jumpDuration / 1000) {
					GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
				}

			}
		}
	}

    /*void jump()
    {
        if (!movementPause)
        {
            if (!keyPressDown)
            {

                keyPressDown = true;
                if (isOnGround || (canDoubleJump && enableDoubleJump || wallHitJump && enableWallJump))
                {
                    bool wallHit = false;
                    int wallHitDirection = 0;

                    if (isOnGround || (canDoubleJump && enableDoubleJump) || leftWall || rightWall)
                    {
                        if (isOnGround == true)
                        {
                           
                            playSound(0, 1.25f);//SOund
                        }
                        else if (!isOnGround && !(leftWall || rightWall))
                        {
                            
                            playSound(0, 0.5f);
                        }
                        else
                        {
                      
                            playSound(0, 0.7f);
                        }
                    }

                    if (leftWall || rightWall)
                    {
                        wallHit = true;
                    }

                    if (horizontal)
                    {
                        if (wallHit)
                        {
                            if (enableWallJump)
                            {
                                if (leftWall)
                                {
                                    wallHitDirection = 1;
                                }
                                else if (rightWall)
                                {
                                    wallHitDirection = -1;
                                }
                            }
                        }
                    }

                    if (!wallHit)
                    {
                        if (isOnGround || (canDoubleJump && enableDoubleJump))
                        {
                            anim.SetInteger("State", 3);
                            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, this.jumpSpeed);
                            jumpDuration = 0.0f;
                            Jumping = true;
                            canDoubleJump = true;
                        }
                    }
                    else
                    {
                        if (isOnGround)
                        {
                            anim.SetInteger("State", 3);
                            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, this.jumpSpeed);
                            jmpDuration = 0.0f;
                            canJumpVariable = true;
                            Jumping = true;
                        }
                        else
                        {
                            if (wallHitDirection != 0)
                            {
                                anim.SetInteger("State", 6);
                                Jumping = true;
                                GetComponent<Rigidbody2D>().velocity = new Vector2(this.jumpSpeed * wallHitDirection, this.jumpSpeed);
                                jmpDuration = 0.0f;
                                canJumpVariable = true;
                            }
                            else
                            {
                               
                            }
                        }


                    }

                    if (!isOnGround && !wallHit)
                    {
                        canDoubleJump = false;
                    }
                }
            }
            else if (canJumpVariable)
            {
                jmpDuration += Time.deltaTime;

                if (jmpDuration < this.jumpDuration / 1000)
                {
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, this.jumpSpeed);
                }
            }
        }
    } */

	void Update ()
	{

        Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
        GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
        leftWall = onLeftWall ();
		rightWall = onRightWall ();
		isOnGround = onGround();
		bool isDashing = invincibility.pollIsDodging ();
		if (isDashing) {
			if (stat != status.dashing) {
				anim.Play ("Dodging", 0, 1);
				stat = status.dashing;
			} else {

			}
		} else {
			if (stat == status.dashing) {
				stat = status.stationary;
			}
		}

		if (!isOnGround && (stat == status.slidingOnWall || stat == status.wallJump || stat == status.standingJump) && (animNeedsFinish <= 0)) {
			stat = status.falling;
		}

		if (animNeedsFinish > 0) {
			animNeedsFinish -= Time.deltaTime;
		} else {
			if (!isOnGround) {
				if (leftWall || rightWall) {
					stat = status.slidingOnWall;
				}
                else if (stat != status.death)
                {
					stat = status.falling;
				}
			}
		}

		if (stat == status.slidingOnWall) {
			//activate sliding anim
			anim.Play ("wallToRightWallSlide" , 0);
            //anim.SetInteger("State", 5);
            stat = status.slidingOnWall;
		} else if (stat == status.falling) {
			//activate falling anim
			anim.Play ("Falling" , 0);
			//anim.SetInteger("State", 3);
			stat = status.falling;
		} else if(stat == status.death)
        {
            anim.Play("death", 0);
            stat = status.death;
        }
        

		if (isOnGround) {
			if (GetComponent<Rigidbody2D> ().velocity.y <= 0) {
				canDoubleJump = true;
				if (GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {
					if (!horizontal) {
						GetComponent<Rigidbody2D> ().velocity = Vector3.Lerp (GetComponent<Rigidbody2D> ().velocity, Vector2.zero, Time.deltaTime * slowDownSpeed);
					}
				}
			} else {
				isOnGround = false;
			}
		}

		if (horizontal == false && isOnGround && !isDashing && stat != status.death)
		{
			//idle
			if (Mathf.Abs(GetComponent<Rigidbody2D> ().velocity.x) < 0.2f || stat == status.falling) {
				anim.Play ("Idle", 0);
				//anim.SetInteger("State", 0);
				stat = status.stationary;
			}
		}

		horizontal = false;
		keyPressDown = false;
		canJumpVariable = false;

        if (stat == status.dodging || stat == status.slidingOnWall || stat == status.wallJump || stat == status.dashing)
        {
            m_attack.SetAttackEnabled(false);
        }
        else
        {
            m_attack.SetAttackEnabled(true);
        }
    }

    private bool onGround()
    {
		//arbitary length for search
        float checkLength = 0.5f;

		//bottom of players position (not relative to rotation)
		Vector2 lineStart = new Vector2(this.transform.position.x, this.transform.position.y - ((GetComponent<SpriteRenderer> ().sprite.rect.size.y * (transform.lossyScale.y / 2)) / 100) + 0.1f);
		//end of seach line
        Vector2 searchVector = new Vector2(this.transform.position.x, lineStart.y - checkLength);
		//linecast all objects that intersect above
		RaycastHit2D[] hitAll = Physics2D.LinecastAll(lineStart, searchVector);
		//debug for search line
		Debug.DrawLine(lineStart, searchVector,Color.green);


		//bottom of players position (not relative to rotation)
		lineStart = new Vector2(this.transform.position.x - rend.bounds.size.x/4, this.transform.position.y - ((GetComponent<SpriteRenderer> ().sprite.rect.size.y * (transform.lossyScale.y / 2)) / 100) + 0.1f);
		//end of seach line
		searchVector = new Vector2(this.transform.position.x - rend.bounds.size.x/4, lineStart.y - checkLength);
		RaycastHit2D[] hitAll2 = Physics2D.LinecastAll(lineStart, searchVector);
		//debug for search line
		Debug.DrawLine(lineStart, searchVector,Color.green);


		//bottom of players position (not relative to rotation)
		lineStart = new Vector2(this.transform.position.x + rend.bounds.size.x/4, this.transform.position.y - ((GetComponent<SpriteRenderer> ().sprite.rect.size.y * (transform.lossyScale.y / 2)) / 100) + 0.1f);
		//end of seach line
		searchVector = new Vector2(this.transform.position.x + rend.bounds.size.x/4, lineStart.y - checkLength);
		RaycastHit2D[] hitAll3 = Physics2D.LinecastAll(lineStart, searchVector);
		//debug for search line
		Debug.DrawLine(lineStart, searchVector,Color.green);


		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hitAll) {
			if (hi.transform != transform) {
				return true;
			}
		}

		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hitAll2) {
			if (hi.transform != transform) {
				return true;
			}
		}

		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hitAll3) {
			if (hi.transform != transform) {
				return true;
			}
		}

		//return false if hit doesn't exists
		return false;
    }

    private bool onLeftWall()
    {

        float checkLength = 0.3f;
        float colliderThreshold = 0.1f;

        Vector2 lineStart = new Vector2(this.transform.position.x - rend.bounds.extents.x - colliderThreshold, this.transform.position.y);
        Vector2 searchVector = new Vector2(lineStart.x - checkLength, this.transform.position.y);
        RaycastHit2D hit = Physics2D.Linecast(lineStart, searchVector);
        Debug.DrawLine(lineStart, searchVector, Color.red);

		lineStart = new Vector2(this.transform.position.x - rend.bounds.extents.x - colliderThreshold, this.transform.position.y - rend.bounds.size.y/3);
		searchVector = new Vector2(lineStart.x - checkLength, this.transform.position.y - rend.bounds.size.y/3);
		RaycastHit2D hit2 = Physics2D.Linecast(lineStart, searchVector);
		Debug.DrawLine(lineStart, searchVector,Color.red);

		return (hit || hit2);
    }

    private bool onRightWall()
    {
       
        float checkLength = 0.3f;
        float colliderThreshold = 0.1f;

        Vector2 lineStart = new Vector2(this.transform.position.x + rend.bounds.extents.x + colliderThreshold, this.transform.position.y);
        Vector2 searchVector = new Vector2(lineStart.x + checkLength, this.transform.position.y);
		RaycastHit2D hit = Physics2D.Linecast(lineStart, searchVector);
		Debug.DrawLine (lineStart, searchVector, Color.red);

		lineStart = new Vector2(this.transform.position.x + rend.bounds.extents.x + colliderThreshold, this.transform.position.y - rend.bounds.size.y/3);
		searchVector = new Vector2(lineStart.x + checkLength, this.transform.position.y - rend.bounds.size.y/3);
		RaycastHit2D hit2 = Physics2D.Linecast(lineStart, searchVector);
        Debug.DrawLine(lineStart, searchVector,Color.red);

		return (hit || hit2);

    }

    public void playSound(int _clip, float defaultVal = 1, float defaultSound = 0.1f)
    {
        GetComponent<AudioSource>().volume = defaultSound;
        GetComponent<AudioSource>().pitch = defaultVal;
       GetComponent<AudioSource>().clip = AudioClip[_clip];
        GetComponent<AudioSource>().Play();
    }

    private void flipAnimation(float flip)
    {
        if (flip > 0 && !facingRight || flip < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 playerScale = transform.localScale;
            playerScale.x *= -1;
            transform.localScale = playerScale;
        }

    }

	float getAnimationTime(string animName) {
		RuntimeAnimatorController animController = anim.runtimeAnimatorController;
		for (int a = 0; a < animController.animationClips.Length; a++) {
			if (animController.animationClips [a].name == animName) {
				return animController.animationClips [a].length;
			}
		}
		return 0;
	}

}

public enum status
{
	stationary,
	walking,
	running,
	standingJump,
	dodging,
	slidingOnWall,
	wallJump,
	death,
	falling,
	dashing
}
