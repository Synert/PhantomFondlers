using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{

    public Transform m_weapon;
    int playerNumber = 1;
    float cooldown = 0.0f;
    bool attacking = false;
    int quad = 0;
    bool quadSet = false;
    bool resetAttack = false;
    float size = 45f;

    float power = 0.0f;
    float powerCap = 1500.0f;
    Vector2 inputs;
    float trigger;

    // Use this for initialization
    void Start()
    {

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
            attacking = false;
            quadSet = false;
        }

        if (m_weapon != null)
        {
            RangedInput(Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg);
        }

        //m_weapon.LookAt(m_weapon.position + inputs);
    }

    void MeleeInput(float angle)
    {
        if (trigger <= 0.1f)
        {
            resetAttack = true;
        }

        if (trigger > 0.1f && cooldown <= 0.0f && m_weapon != null && resetAttack)
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
            power += 1000.0f * Time.deltaTime;
            if (power >= powerCap)
            {
                power = powerCap;
                cooldown = 250.0f;
            }
            if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
                m_weapon.GetComponentInChildren<WeaponPickup>().ChargeAngle(90.0f - angle, power);
            }
            else
            {
                float newAngle = quad * size;
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);
                m_weapon.GetComponentInChildren<WeaponPickup>().ChargeDirection(quad, power);
            }
            //Debug.DrawLine(transform.position, transform.position + (Vector3)inputs * 5.0f, Color.blue);
        }
        else if (attacking)
        {
            attacking = false;
            resetAttack = false;
            cooldown = m_weapon.GetComponentInChildren<WeaponPickup>().GetAttackTime();
            quadSet = false;

            Vector3 prevPos = transform.position;

            float newAngle = quad * size;
            if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
                m_weapon.GetComponentInChildren<WeaponPickup>().AttackAngle(90.0f - angle, power);
                //Debug.DrawLine(prevPos, transform.position + Quaternion.AngleAxis(90.0f - angle, Vector3.forward) * transform.right * 2.0f, Color.yellow, 1.0f);
            }
            else
            {
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);

                m_weapon.GetComponentInChildren<WeaponPickup>().AttackDirection(quad, power);
                //Debug.DrawLine(prevPos, transform.position + Quaternion.AngleAxis(newAngle, Vector3.forward) * transform.right * 2.0f, Color.yellow, 1.0f);
            }

            power = 0.0f;
        }
    }

    void RangedInput(float angle)
    {
        if (inputs.magnitude <= 0.75f)
        {
            //resetAttack = true;
        }

        if (inputs.magnitude > 0.75f)
        {
            if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
                //Debug.DrawLine(transform.position, transform.position + (Vector3)inputs * 3.0f, Color.blue);
            }
            else
            {
                float newAngle = quad * size;
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);
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
                GetComponent<PlayerController>().playSound(1);
            }
            if (!quadSet)
            {
                quad = GetQuadrant(angle, size);
                quadSet = true;
                //m_weapon.GetComponentInChildren<WeaponPickup>().Charge(quad);
            }
            power += 1000.0f * Time.deltaTime;
            if (power >= powerCap)
            {
                power = powerCap;
                cooldown = 250.0f;
            }
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
        else if (attacking)
        {
            attacking = false;
            resetAttack = false;
            cooldown = m_weapon.GetComponentInChildren<WeaponPickup>().GetAttackTime();
            quadSet = false;

            Vector3 prevPos = transform.position;

            float newAngle = quad * size;
            if (!m_weapon.GetComponentInChildren<WeaponPickup>().GetAngleLock())
            {
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
                m_weapon.GetComponentInChildren<WeaponPickup>().AttackAngle(90.0f - angle, power);
                //Debug.DrawLine(prevPos, transform.position + Quaternion.AngleAxis(90.0f - angle, Vector3.forward) * transform.right * 2.0f, Color.yellow, 1.0f);
            }
            else
            {
                //m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, newAngle - 90.0f);

                m_weapon.GetComponentInChildren<WeaponPickup>().AttackDirection(quad, power);
                //Debug.DrawLine(prevPos, transform.position + Quaternion.AngleAxis(newAngle, Vector3.forward) * transform.right * 2.0f, Color.yellow, 1.0f);
            }

            power = 0.0f;
        }
    }

    public void SwitchWeapon(Transform new_weapon)
    {

        Quaternion oldRot = m_weapon.rotation;
        Transform old_weapon = Instantiate(m_weapon);
        old_weapon.localScale = new_weapon.localScale;
        old_weapon.position = new_weapon.position;
        old_weapon.GetComponent<WeaponPickup>().Swapped();
        old_weapon.localRotation = Quaternion.identity;
        Destroy(m_weapon.gameObject);
        m_weapon = Instantiate(new_weapon, transform);
        m_weapon.GetComponent<WeaponPickup>().SetOwner(null);
        m_weapon.localPosition = Vector3.zero;
        m_weapon.localScale = Vector3.one;
        m_weapon.GetComponent<WeaponPickup>().Swapped();
        m_weapon.rotation = oldRot;
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