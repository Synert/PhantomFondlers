using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Renderer rend = new Renderer();
    public float MaxSpeed = 6;
    public float Acceleration = 5;
    public float jumpSpeed = 5;
    public float jumpDuration;
	public bool isOnGround = false;
	public float slowDownSpeed;

	public bool rotateAroundObject = true;
	public float rotationSpeed = 1;
	public float resetJumpHeight = 1;
	public Vector2 MinMaxZRot = new Vector2();

    public bool enableWallJump = true;
    public bool enableDoubleJump = true;
    public bool wallHitJump = true;

	//===== Data for buttonMashing ====//
	public float arbitaryHealth = 0;
	public PlayerController ontopOf;
	public float increaseCount = 0;
	public float increaseWhenOntopCount = 0;
	public float currentCount = 0;
	public float respawnCount = 100;
	public float respawnHealth = 10;
	//=================================

    bool canDoubleJump = true;
    float jmpDuration;

    bool keyPressDown = false;
    bool canJumpVariable = false; 

	bool horizontal = false;
	public bool movementPause = false;

    void Start()
    {

		rend = GetComponent<Renderer>();
		GetComponent<BoxCollider2D> ().size = (GetComponent<SpriteRenderer> ().sprite.rect.size / 100);

    }

	//==============================================
	//Deal with buttonMashing to revive/tickle to death
	//needs to be polished but fundementals are done
	void takeDamage() {
		if (!movementPause) {
			arbitaryHealth -= 1;
			if (arbitaryHealth <= 0) {
				movementPause = true;
			}
		}
	}

	//if health below 0 count up
	//if person currently ontop count up slower
	//if health above 0 and above another
	//if other health below 0 count down other
	void buttonMash () {
		if (arbitaryHealth <= 0) {
			if (!movementPause) {
				movementPause = true;//debug
			}
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
		} else if (currentCount >= respawnCount) {
			movementPause = false;
			arbitaryHealth = respawnHealth;
			currentCount = 0;
		}
	}

	//if other has no ontop set ontop of other and self to required
	void OnTriggerEnter2D (Collider2D other) {
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
	void OnTriggerExit2D (Collider2D other) {
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

	void inputs(variableData variable) {
		if (!movementPause) {
			if (variable.axisValue < 0) {
				if (GetComponent<Rigidbody2D> ().velocity.x > -this.MaxSpeed) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-this.Acceleration, 0.0f));
				}
			} else {
				if (GetComponent<Rigidbody2D> ().velocity.x < this.MaxSpeed) {
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (this.Acceleration, 0.0f));
				}
			}
			horizontal = true;
		}
	}

	void jump() {
		if (!movementPause) {
			if (!keyPressDown) {
				keyPressDown = true;
				if (isOnGround || (canDoubleJump && enableDoubleJump || wallHitJump && enableWallJump)) {
					bool wallHit = false;
					int wallHitDirection = 0;

					bool leftWallHit = onLeftWall ();
					bool rightWallHit = onRightWall ();

					if (horizontal) {
						if (leftWallHit && enableWallJump) {
							wallHit = true;
							wallHitDirection = 1;
						} else if (rightWallHit && enableWallJump) {
							wallHit = true;
							wallHitDirection = -1;
						}
					}

					if (!wallHit) {
						if (isOnGround || (canDoubleJump && enableDoubleJump)) {
							GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, this.jumpSpeed);
							jumpDuration = 0.0f;
							canDoubleJump = true;
						}
					} else {
						GetComponent<Rigidbody2D> ().velocity = new Vector2 (this.jumpSpeed * wallHitDirection, this.jumpSpeed);

						jmpDuration = 0.0f;
						canJumpVariable = true;
					}

					if (!isOnGround && !wallHit) {
						canDoubleJump = false;
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

	// Update is called once per frame
	void Update ()
    {

        isOnGround = onGround();

        if (isOnGround)
        {
            canDoubleJump = true;
			if (GetComponent<Rigidbody2D> ().velocity != Vector2.zero) {
				if (!horizontal) {
					GetComponent<Rigidbody2D> ().velocity = Vector3.Lerp (GetComponent<Rigidbody2D> ().velocity, Vector2.zero, Time.deltaTime * slowDownSpeed);
				}
			}
        }

        if (Input.GetKeyDown("space"))
        {

        }

		horizontal = false;
		keyPressDown = false;
		canJumpVariable = false;
    }

    private bool onGround()
    {
		//arbitary length for search
        float checkLength = 0.5f;

		//bottom of players position (not relative to rotation)
		Vector2 lineStart = new Vector2(this.transform.position.x, this.transform.position.y - ((GetComponent<SpriteRenderer> ().sprite.rect.size.y * (transform.lossyScale.y / 2)) / 100));

		//end of seach line
        Vector2 searchVector = new Vector2(this.transform.position.x, lineStart.y - checkLength);

		//linecast all objects that intersect above
		RaycastHit2D[] hitAll = Physics2D.LinecastAll(lineStart, searchVector);

		//loop through all hits and return first object that isn't self
		RaycastHit2D hit = new RaycastHit2D();
		foreach (RaycastHit2D hi in hitAll) {
			if (hi.transform != transform) {
				hit = hi;
				break;
			}
		}

		//debug for search line
        Debug.DrawLine(lineStart, searchVector,Color.green);

        //deal with rotating the character to match the object it is standing on
		if (rotateAroundObject) {
			//check if raycast hit anyting
			if (hit) {
				
				//get hit rotation and bring it in to search space
				//makes it within -180 -> 180
				Vector3 rot = hit.transform.rotation.eulerAngles;
				while (rot.z < -180 || rot.z > 180) {
					if (rot.z > 180) {
						rot.z -= 180;
					} else if (rot.z < -180) {
						rot.z += 180;
					}
				}

				//check if rotation is within accepted limits
				if (rot.z > MinMaxZRot.x &&
					rot.z < MinMaxZRot.y) {
					//setup vector 3 for current rotation + hit's
					Vector3 tempEular = transform.rotation.eulerAngles;
					tempEular.z = rot.z;

					//translate above position into quaternion and set rotation
					Quaternion temp = Quaternion.Euler(tempEular);
					transform.rotation = temp;
				}
				else {
					//if hit rotation to large reset rotation
					Quaternion temp = transform.rotation;
					temp.z = 0;
					transform.rotation = temp;
				}
			}
			else {
				//if no hit reset rotation
				Quaternion temp = transform.rotation;
				temp.z = 0;
				transform.rotation = temp;
			}
		}

		//return true if hit exists
		return hit;
    }

    private bool onLeftWall()
    {

        float checkLength = 0.2f;
        float colliderThreshold = 0.1f;

        Vector2 lineStart = new Vector2(this.transform.position.x - rend.bounds.extents.x - colliderThreshold, this.transform.position.y);

        Vector2 searchVector = new Vector2(lineStart.x - checkLength, this.transform.position.y);

        RaycastHit2D hit = Physics2D.Linecast(lineStart, searchVector);
        Debug.DrawLine(lineStart, searchVector, Color.red);

        return hit;
    }

    private bool onRightWall()
    {
       
        float checkLength = 0.2f;
        float colliderThreshold = 0.1f;

        Vector2 lineStart = new Vector2(this.transform.position.x + rend.bounds.extents.x + colliderThreshold, this.transform.position.y);
       

        Vector2 searchVector = new Vector2(lineStart.x + checkLength, this.transform.position.y);

        RaycastHit2D hit = Physics2D.Linecast(lineStart, searchVector);
        Debug.DrawLine(lineStart, searchVector,Color.red);
        return hit;

    }

}
