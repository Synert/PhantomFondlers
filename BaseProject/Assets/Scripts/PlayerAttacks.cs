﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    Transform m_weapon = null;
    public Transform m_duster;
    float cooldown = 0.0f;
    bool attacking = false;
    int quad = 0;
    bool quadSet = false;
    bool resetAttack = false;
    float size = 45f;
    public bool canAttack = true;

    float power = 0.0f;
    Vector2 inputs;
    float trigger;

    // Use this for initialization
    void Start()
    {
        if(m_weapon != null)
        {
            m_weapon.GetComponentInChildren<WeaponPickup>().SetOwner(gameObject);
        }
        m_duster.GetComponentInChildren<WeaponPickup>().SetOwner(gameObject);
        m_duster.transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));
    }

    void grabInputs(variableData _var)
    {
        inputs = _var.state.ThumbStickRight.inputs;
        //trigger = _var.state.TriggerRight.input;
        //inputs.y = -inputs.y;
    }

    void grabTrigger(variableData _var)
    {
        trigger = _var.state.TriggerRight.input;
    }

    // Update is called once per frame
    void Update()
    {

        if (inputs.magnitude > 1.0f)
        {
            //inputs.Normalize();
        }

        if (cooldown > 0.0f)
        {
            cooldown -= 1000.0f * Time.deltaTime;
        }

        if (canAttack)
        {

            if (m_weapon == null)
            {
                m_duster.gameObject.SetActive(true);
            }
            else
            {
                m_duster.gameObject.SetActive(false);
                m_weapon.gameObject.SetActive(true);
            }

            Input(Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg);
        }

        else
        {
            //if(m_weapon != null)
            //{
            //    m_weapon.gameObject.SetActive(false);
            //}
            //m_duster.gameObject.SetActive(false);
            attacking = false;
            power = 0.0f;
            quadSet = false;
        }
    }

    void Input(float angle)
    {

        if (inputs.magnitude > 0.3f)
        {
            if (m_weapon != null && !m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
            }
            else if (m_duster != null && !m_duster.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                m_duster.eulerAngles = new Vector3(m_duster.eulerAngles.x, m_duster.eulerAngles.y, -angle);
            }
        }

        if(trigger <= 0.1f)
        {
            resetAttack = true;
        }

        if (trigger > 0.1f && cooldown <= 0.0f && resetAttack)
        {
            if (!attacking)
            {
                attacking = true;
            }
            if (!quadSet)
            {
                quad = GetQuadrant(angle, size);
                quadSet = true;
                //m_weapon.GetComponentInChildren<WeaponPickup>().Charge(quad);
            }
            power += Time.deltaTime;
            float powerCap = 0.0f;

            if(m_weapon != null)
            {
                powerCap = m_weapon.GetComponentInChildren<WeaponPickup>().powerCap;
            }
            else
            {
                powerCap = m_duster.GetComponentInChildren<WeaponPickup>().powerCap;
            }

            if (power >= powerCap)
            {
                power = powerCap;
                cooldown = 250.0f;
            }

            if (m_weapon != null)
            {
                if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
                {
                    m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
                }
                else
                {
                    float newAngle = quad * size;
                    m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);
                }
                m_weapon.GetComponentInChildren<WeaponPickup>().Charge(power);
            }
            else
            {
                if (!m_duster.GetComponentInChildren<WeaponPickup>().GetAngleLock())
                {
                    m_duster.eulerAngles = new Vector3(m_duster.eulerAngles.x, m_duster.eulerAngles.y, -angle);
                }
                else
                {
                    float newAngle = quad * size;
                    m_duster.eulerAngles = new Vector3(m_duster.eulerAngles.x, m_duster.eulerAngles.y, newAngle - 90.0f);
                }
                m_duster.GetComponentInChildren<WeaponPickup>().Charge(power);
            }
        }
        else if (attacking)
        {
            GetComponent<PlayerController>().playSound(1);
            attacking = false;
            resetAttack = false;
            if(m_weapon != null)
            {
                cooldown = m_weapon.GetComponentInChildren<WeaponPickup>().GetAttackTime();
            }
            else
            {
                cooldown = m_duster.GetComponentInChildren<WeaponPickup>().GetAttackTime();
            }
            quadSet = false;

            Vector3 prevPos = transform.position;

            float newAngle = quad * size;
            if (m_weapon != null)
            {
                m_weapon.GetComponentInChildren<WeaponPickup>().Attack(power);
            }
            else
            {
                m_duster.GetComponentInChildren<WeaponPickup>().Attack(power);
            }

            power = 0.0f;
        }
    }

    public void SwitchWeapon(Transform new_weapon)
    {
        if(m_weapon != null)
        {
            string oldName = m_weapon.name;
            Quaternion oldRot = m_weapon.rotation;
            Transform old_weapon = Instantiate(m_weapon);
            old_weapon.localScale = new_weapon.localScale;
            old_weapon.position = new_weapon.position;
            old_weapon.GetComponent<WeaponPickup>().Swapped();
            old_weapon.localRotation = Quaternion.identity;
            old_weapon.name = oldName;
            Destroy(m_weapon.gameObject);

            old_weapon.GetComponent<WeaponPickup>().SetOwner(null);
        }
        string newName = new_weapon.name;
        m_weapon = Instantiate(new_weapon, transform);
        m_weapon.GetComponent<WeaponPickup>().SetOwner(null);
        m_weapon.localPosition = Vector3.zero;
        m_weapon.localScale = Vector3.one;
        m_weapon.GetComponent<WeaponPickup>().Swapped();
        //m_weapon.rotation = oldRot;
        m_weapon.name = newName;
        Destroy(new_weapon.gameObject);

        m_weapon.GetComponent<WeaponPickup>().SetOwner(gameObject);
    }

    int GetQuadrant(float angle, float size)
    {
        int maxCounter = (int)Mathf.Round(360.0f / size);
        int start = (int)Mathf.Round(maxCounter / 4);
        int counter = 0;
        angle += (size / 2.0f);
        if (angle < 0)
        {
            angle += 360.0f;
        }
        if (angle > 360)
        {
            angle -= 360.0f;
        }

        int ret = 0;

        while ((counter + 1) * size <= 360)
        {
            if (angle >= counter * size && angle < (counter + 1) * size)
            {
                ret = (start - counter);
                if (ret >= maxCounter)
                {
                    ret -= maxCounter;
                }
                if (ret < 0)
                {
                    ret += maxCounter;
                }
                return ret;
            }
            counter++;
        }

        return ret;
    }

    public void SetAttackEnabled(bool set)
    {
        canAttack = set;
		if (m_weapon) {
			toggleWeaponRender (m_weapon, set);
		}
		if (m_duster) {
			toggleWeaponRender (m_duster, set);
		}
    }

	void toggleWeaponRender(Transform objParent, bool set) {
		objParent.GetComponent<WeaponPickup> ().sprite.GetComponent<SpriteRenderer> ().enabled = set;
	}

    public bool CanAttack()
    {
        return canAttack;
    }

    public void SetColor(Color set)
    {
        foreach(SpriteRenderer sprite in m_duster.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = set;
        }
        if(m_weapon)
        {
            foreach (SpriteRenderer sprite in m_weapon.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = set;
            }
        }
    }

    public bool HasWeapon()
    {
        return !(m_weapon == null);
    }
}