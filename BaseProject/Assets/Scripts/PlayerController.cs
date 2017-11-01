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

    public Color playerColor = Color.white;

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
	public float currentCount = 0;
	public float currentEnemyCount = 0;
	public float respawnCount = 100;
	public float enemyKillCount = 100;
	public float respawnHealth = 10;
    //=================================
    //====== Data for sprinting ======//
    public float sprintSpeedModifier;
    //=================================
	//temp
	public status stat;
	public float animNeedsFinish = 0;
	public GameObject ghostChild;
	public GameObject XChild;
	public int downMult = 0;
	public int downs = 0;
	public Rect counterOntop;
	public Rect counterDowned;
	public GameObject counterBox;
	public GameObject counter;
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

        //originalSize = GetComponent<Collider2D>().bounds.size;
        //Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
        //GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
    }

	//==============================================
	//Deal with buttonMashing to revive/tickle to death
	//needs to be polished but fundementals are done
	public void takeDamage(int damage) {
		if (!movementPause && stat != status.dodging && stat != status.dashing) {
			arbitaryHealth -= damage;
			if (arbitaryHealth <= 0)
			{
				downs++;
				stat = status.death;
				anim.Play("death", 0);
				animNeedsFinish = getAnimationTime("death", anim);
				//GetComponent<BoxCollider2D>().isTrigger = true;
				GetComponent<Rigidbody2D>().mass = 100;
				movementPause = true;
				Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
				GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
				if (ontopOf) {
					ontopOf.ontopOf = null;
					ontopOf = null;
				}
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
		XChild.GetComponent<xButtonAnim> ().animEnum = buttonAnim.pressed;
		if (arbitaryHealth <= 0) {
			GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			if (currentCount < (respawnCount + (downMult * downs))) {
				currentCount += increaseCount;
			}
			testHealth ();
		} else {
			if (ontopOf) {
				if (ontopOf.arbitaryHealth <= 0) {
					if (ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.resurrect &&
						ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.disable) {
						ghostChild.GetComponent<ghostAnimator> ().animEnum = ghostAnim.buttonMash;
					}
					GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
					movementPause = true;
					if (ontopOf.currentEnemyCount < ontopOf.enemyKillCount) {
						ontopOf.currentEnemyCount += increaseCount;
					}
					ontopOf.testHealth ();
				}
			}
		}
	}

	void exitButtonMash () {
		if (ontopOf) {
			if (arbitaryHealth > 0) {
				ontopOf.ontopOf = null;
				ontopOf = null;
				movementPause = false;
				//========================================ADD OTHER NEEDED DATA
			}
		}
	}

	//check if arbitary health is 
	//below 0 or if currenCount is
	//above respawn and take actions
	public void testHealth() {
		if (currentEnemyCount >= enemyKillCount) {
			ontopOf.movementPause = false;
			ontopOf.ontopOf = null;
			XChild.gameObject.SetActive (false);
			ontopOf.XChild.SetActive(false);
			ontopOf = null;
			//Destroy (this.gameObject);
			if (ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.die &&
				ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.dieDisable) {
				ghostChild.GetComponent<ghostAnimator> ().animEnum = ghostAnim.die;
			}
		} else if (currentCount >= (respawnCount + (downMult * downs)))
		{
			if (ontopOf) {
				ontopOf.movementPause = false;
				ontopOf.ontopOf = null;
				XChild.gameObject.SetActive (false);
				ontopOf.XChild.SetActive(false);
				ontopOf = null;
			}
			if (ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.resurrect &&
				ghostChild.GetComponent<ghostAnimator> ().animEnum != ghostAnim.disable) {
				ghostChild.GetComponent<ghostAnimator> ().animEnum = ghostAnim.resurrect;
			}
		}
	}

	public void resurrect() {
		stat = status.stationary;
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<BoxCollider2D> ().isTrigger = false;
		GetComponent<Rigidbody2D>().mass = 1;
		movementPause = false;
		arbitaryHealth = respawnHealth;
		currentCount = 0;
	}

	public void forceKill() {
		Destroy (this.gameObject);
	}

	//if other has no ontop set ontop of other and self to required
	void OnTriggerEnter2D (Collider2D other) {
		if (other.GetComponent<PlayerController> ()) {
			if (other.GetComponent<PlayerController> ().arbitaryHealth <= 0) {
				if (!movementPause) {
					if (!other.GetComponent<PlayerController> ().ontopOf) {
						ontopOf = other.GetComponent<PlayerController> ();
						other.GetComponent<PlayerController> ().ontopOf = this;
						XChild.gameObject.SetActive (true);
						other.GetComponent<PlayerController> ().XChild.SetActive(true);
					}
				}
			}
		}
	}

	//if other has no ontop set ontop of other and self to required
	void OnTriggerStay2D (Collider2D other) {
		if (other.GetComponent<PlayerController> ()) {
			if (other.GetComponent<PlayerController> ().arbitaryHealth <= 0) {
				if (!movementPause) {
					if (!other.GetComponent<PlayerController> ().ontopOf) {
						if (!ontopOf) {
							ontopOf = other.GetComponent<PlayerController> ();
							other.GetComponent<PlayerController> ().ontopOf = this;
							XChild.gameObject.SetActive (true);
							other.GetComponent<PlayerController> ().XChild.SetActive (true);
						}
					}
				}
			}
		}
	}

	//if other has ontop set ontop of other and self to required
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.GetComponent<PlayerController> ()) {
			if (other.GetComponent<PlayerController> ().ontopOf) {
				if (other.GetComponent<PlayerController> ().ontopOf == this) {
					if (ontopOf) {
						ontopOf.currentEnemyCount = 0;
						ontopOf = null;
					}
					XChild.gameObject.SetActive (false);
					other.GetComponent<PlayerController> ().XChild.gameObject.SetActive (false);
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
								animNeedsFinish = getAnimationTime("WallJump");
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

	void Update ()
	{
        GetComponent<SpriteRenderer>().color = playerColor;
        m_attack.SetColor(playerColor);
        //Vector2 temp = GetComponent<SpriteRenderer>().bounds.size;
        //GetComponent<Collider2D>().bounds.size.Set(temp.x, temp.y, 0);
        leftWall = onLeftWall ();
		rightWall = onRightWall ();
		isOnGround = onGround();
		bool isDashing = invincibility.pollIsDodging ();
		if (isDashing) {
			if (stat != status.dashing) {
				anim.Play ("Dodging", 0, 1);
                animNeedsFinish = getAnimationTime("Dodging");
                stat = status.dashing;
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
				if (stat != status.death && stat != status.deathAnim) {
					if (leftWall || rightWall) {
						stat = status.slidingOnWall;
					} else {
						stat = status.falling;
					}
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
		} else if(stat == status.death || stat == status.deathAnim)
		{
			anim.Play("death", 0);
		}
        
		//handle death positioning
		if (stat == status.death) {
			if (isOnGround) {
				GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				GetComponent<Rigidbody2D> ().isKinematic = true;
				GetComponent<BoxCollider2D> ().isTrigger = true;
				transform.position -= new Vector3 (0, 0.6f, 0);
				Vector3 localScale = ghostChild.transform.localScale;
				if (!facingRight) {
					ghostChild.transform.position = new Vector3 (transform.position.x + 0.9f, ghostChild.transform.position.y, transform.position.z);
					XChild.transform.position = new Vector3 (transform.position.x - 0.9f, ghostChild.transform.position.y, transform.position.z);
					localScale.x = Mathf.Abs (ghostChild.transform.localScale.x);
				} else {
					localScale.x = ghostChild.transform.localScale.x;
					XChild.transform.position = new Vector3 (transform.position.x + 0.9f, ghostChild.transform.position.y, transform.position.z);
					ghostChild.transform.position = new Vector3 (transform.position.x - 0.9f, ghostChild.transform.position.y, transform.position.z);
				}
				ghostChild.transform.localScale = localScale;
				ghostChild.SetActive (true);
				ghostChild.GetComponent<ghostAnimator> ().animEnum = ghostAnim.appear;
				stat = status.deathAnim;
			}
		} else if (stat != status.death && stat != status.deathAnim) {
			XChild.transform.position = new Vector3 (transform.position.x, transform.position.y + 1.25f, transform.position.z);
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

		if (horizontal == false && isOnGround && !isDashing && (stat != status.death && stat != status.deathAnim))
		{
			//idle
			if (Mathf.Abs(GetComponent<Rigidbody2D> ().velocity.x) < 0.2f || stat == status.falling) {
				anim.Play ("Idle", 0);
				//anim.SetInteger("State", 0);
				stat = status.stationary;
			}
		}

		//draw health ui
		if (ontopOf) {
			counterBox.SetActive (true);
			counter.SetActive (true);
			if (arbitaryHealth <= 0) {
				drawCounterBar (currentCount, respawnCount + (downMult * downs), counterDowned);
			} else {
				drawCounterBar (ontopOf.currentEnemyCount, ontopOf.enemyKillCount, counterOntop);
			}
		} else if (arbitaryHealth <= 0) {
			counterBox.SetActive (true);
			counter.SetActive (true);
			drawCounterBar (currentCount, respawnCount + (downMult * downs), counterDowned);
		} else {
			counterBox.SetActive (false);
			counter.SetActive (false);
		}

		horizontal = false;
		keyPressDown = false;
		canJumpVariable = false;

		if (stat == status.dodging || stat == status.slidingOnWall || stat == status.wallJump || stat == status.dashing || stat == status.death  || stat == status.deathAnim)
        {
            m_attack.SetAttackEnabled(false);
        }
        else
        {
            m_attack.SetAttackEnabled(true);
        }
    }

	void drawCounterBar(float current, float max, Rect _rect) {
		Vector3 pos = new Vector3(_rect.position.x,_rect.position.y, 0);

		counterBox.transform.position = pos + transform.position;
		counterBox.transform.localScale = new Vector3 (_rect.size.x, _rect.size.y, 1);
		pos -= new Vector3 (0, 0, 1);
		counter.transform.position = pos + transform.position;
		counter.transform.localScale = new Vector3 ((_rect.size.x * ((current + 1)/max)) - 0.1f, _rect.size.y - 0.1f, 1);

		//GUI.Box (new Rect(pos + _rect.position, _rect.size), counter);
		//GUI.Box(new Rect(pos + _rect.position, new Vector2(_rect.size.x * (max/current),_rect.size.y)), GUIContent.none);
	}

	private bool onGround()
	{
		//arbitary length for search
		Vector3 size = GetComponent<Collider2D> ().bounds.size;

		//bottom of players position (not relative to rotation)
		Vector2 lineStart = new Vector2(this.transform.position.x - (size.x/2) + 0.1f, this.transform.position.y - (size.y/2) - 0.1f);

		//end of seach line
		Vector2 searchVector = new Vector2(this.transform.position.x + (size.x/2) - 0.1f, this.transform.position.y - (size.y/2) - 0.1f);

		//linecast all objects that intersect above
		RaycastHit2D[] hit = Physics2D.LinecastAll(lineStart, searchVector);

		//debug for search line
		Debug.DrawLine(lineStart, searchVector,Color.green);

		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hit) {
			if (hi.transform != transform) {
				if (hi.transform.tag == "Player") {
					if (!hi.transform.GetComponent<PlayerController> ().movementPause) {
						return true;
					}
				} else {
					return true;
				}
			}
		}

		//return false if hit doesn't exists
		return false;
	}

	private bool onLeftWall()
	{
		Vector3 size = GetComponent<Collider2D> ().bounds.size;

		Vector2 lineStart = new Vector2(this.transform.position.x - (size.x/2) - 0.1f, this.transform.position.y + (size.y/4));
		Vector2 searchVector = new Vector2(this.transform.position.x - (size.x/2) - 0.1f, this.transform.position.y - (size.y/2) + 0.1f);
		RaycastHit2D[] hit = Physics2D.LinecastAll(lineStart, searchVector);
		Debug.DrawLine(lineStart, searchVector,Color.red);

		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hit) {
			if (hi.transform != transform) {
				if (hi.transform.tag == "Player") {
					if (!hi.transform.GetComponent<PlayerController> ().movementPause) {
						return true;
					}
				} else {
					return true;
				}
			}
		}

		return false;
	}

	private bool onRightWall()
	{

		Vector3 size = GetComponent<Collider2D> ().bounds.size;

		Vector2 lineStart = new Vector2(this.transform.position.x + (size.x/2) + 0.1f, this.transform.position.y + (size.y/4));
		Vector2 searchVector = new Vector2(this.transform.position.x + (size.x/2) + 0.1f, this.transform.position.y - (size.y/2) + 0.1f);
		RaycastHit2D[] hit = Physics2D.LinecastAll(lineStart, searchVector);
		Debug.DrawLine(lineStart, searchVector,Color.red);

		//loop through all hits and return first object that isn't self
		foreach (RaycastHit2D hi in hit) {
			if (hi.transform != transform) {
				if (hi.transform.tag == "Player") {
					if (!hi.transform.GetComponent<PlayerController> ().movementPause) {
						return true;
					}
				} else {
					return true;
				}
			}
		}

		return false;

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

	static public float getAnimationTime(string animName, Animator anim) {
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
	dashing,
	deathAnim
}
