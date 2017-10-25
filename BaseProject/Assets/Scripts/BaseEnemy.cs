using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    int direction = 1;
    Rigidbody2D m_rigidbody;
    float acceleration = 10.0f;
    float maxSpeed = 3.5f;
    float bounceCooldown = 0.0f;

	// Use this for initialization
	void Start () {
        m_rigidbody = GetComponentInChildren<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        bool onGround = false;

        if(Physics2D.Raycast(transform.position - transform.up * 0.55f, -transform.up, 0.05f))
        {
            onGround = true;
        }
        if (onGround)
        {
            if (m_rigidbody.velocity.x < maxSpeed && direction == 1)
            {
                m_rigidbody.AddForce(direction * transform.right * acceleration);
            }
            else if (m_rigidbody.velocity.x > -maxSpeed && direction == -1)
            {
                m_rigidbody.AddForce(direction * transform.right * acceleration);
            }
        }

        if(m_rigidbody.velocity.x > maxSpeed)
        {
            m_rigidbody.velocity = new Vector2(maxSpeed, m_rigidbody.velocity.y);
        }

        if (m_rigidbody.velocity.x < -maxSpeed)
        {
            m_rigidbody.velocity = new Vector2(-maxSpeed, m_rigidbody.velocity.y);
        }

        Debug.DrawRay(transform.position + direction * transform.right * 0.55f, direction * transform.right * 0.85f);
		if(Physics2D.Raycast(transform.position + direction * transform.right * 0.55f, direction * transform.right, 0.85f))
        {
            Debug.Log("hit wall");
            direction *= -1;
        }
        else if (!Physics2D.Raycast(transform.position + direction * transform.right * 1f, -transform.up, 0.85f) && onGround)
        {
            Debug.Log("detected gap");
            direction *= -1;
        }
        else if (Physics2D.Raycast(transform.position + direction * (transform.right * 0.55f) - (transform.up * 0.45f), direction * transform.right, 0.95f))
        {
            Debug.Log("hit low wall");
            m_rigidbody.AddForce(transform.up * 20.0f);
        }

        if(bounceCooldown > 0.0f)
        {
            bounceCooldown -= 1000.0f * Time.deltaTime;
        }

        if (!Physics2D.Raycast(transform.position + -direction * transform.right * 0.55f, -transform.up, 0.95f) && bounceCooldown <= 0.0f && onGround)
        {
            m_rigidbody.velocity = new Vector2(-m_rigidbody.velocity.x * 0.75f, m_rigidbody.velocity.y);
            bounceCooldown = 100.0f;
        }
        Debug.DrawRay(transform.position + direction * (transform.right * 0.55f) - (transform.up * 0.45f), direction * transform.right * 0.85f, Color.cyan);
        Debug.DrawRay(transform.position + direction * transform.right * 1f, -transform.up * 0.95f, Color.green);
        Debug.DrawRay(transform.position - direction * transform.right * 0.55f, -transform.up * 0.95f, Color.magenta);
    }
}
