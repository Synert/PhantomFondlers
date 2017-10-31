using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    Transform m_weapon = null;
    public Transform m_duster;
    int playerNumber = 1;
    float cooldown = 0.0f;
    bool attacking = false;
    int quad = 0;
    bool quadSet = false;
    bool resetAttack = false;
    float size = 45f;

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

        if (m_weapon == null)
        {
            m_duster.gameObject.SetActive(true);
        }
        else
        {
            m_duster.gameObject.SetActive(false);
        }

        Input(Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg);
    }

    void Input(float angle)
    {

        if (inputs.magnitude > 0.75f)
        {
            if (m_weapon != null && !m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
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
                    m_weapon.GetComponentInChildren<WeaponPickup>().ChargeAngle(90.0f - angle, power);
                }
                else
                {
                    float newAngle = quad * size;
                    m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);
                    m_weapon.GetComponentInChildren<WeaponPickup>().ChargeDirection(quad, power);
                }
            }
            else
            {
                float newAngle = quad * size;
                m_duster.eulerAngles = new Vector3(m_duster.eulerAngles.x, m_duster.eulerAngles.y, newAngle - 90.0f);
                m_duster.GetComponentInChildren<WeaponPickup>().ChargeDirection(quad, power);
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
                if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
                {
                    m_weapon.GetComponentInChildren<WeaponPickup>().AttackAngle(90.0f - angle, power);
                }
                else
                {
                    m_weapon.GetComponentInChildren<WeaponPickup>().AttackDirection(quad, power);
                }
            }
            else
            {
                m_duster.GetComponentInChildren<WeaponPickup>().AttackDirection(quad, power);
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

    public void SetPlayerNumber(int set)
    {
        playerNumber = set;
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
}