using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour {

    public Transform m_weapon;
    int playerNumber = 1;
    float cooldown = 0.0f;
    bool drawing = false;
    float drawTime = 0.0f;
    List<Vector2> posList;
    int[] quadList = new int[8];
    int curQuads = 0;
    int lastQuad = -1;
    float size = 30.0f;
    int quadLimit = 5;

    // Use this for initialization
    void Start () {
        posList = new List<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 inputs;
        inputs.x = Input.GetAxis("AttackHorz" + playerNumber);
        inputs.y = -Input.GetAxis("AttackVert" + playerNumber);

        if (inputs.magnitude > 1.0f)
        {
            inputs.Normalize();
        }

        if (cooldown > 0.0f)
        {
            cooldown -= 1000.0f * Time.deltaTime;
        }

        if (inputs.magnitude > 0.5f && cooldown <= 0.0f)
        {
            float angle = Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg;
            if (!drawing)
            {
                drawing = true;
                posList.Clear();
            }
            if(lastQuad == -1)
            {
                Debug.Log(curQuads + " " + Time.time);
                quadList[curQuads] = GetQuadrant(angle, size);
                lastQuad = quadList[curQuads];
                curQuads++;
            }
            else
            {
                if(GetQuadrant(angle, size) != lastQuad)
                {
                    quadList[curQuads] = GetQuadrant(angle, size);
                    lastQuad = quadList[curQuads];
                    curQuads++;
                }
                if(curQuads >= quadLimit)
                {
                    cooldown = 250.0f;
                }
            }
            m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, -angle);
            posList.Add(inputs);

            Vector2 prevPos = posList[0];
            foreach(Vector2 pos in posList)
            {
                Debug.DrawLine((Vector2)transform.position + prevPos * 2.0f, (Vector2)transform.position + pos * 2.0f, Color.blue, 0.5f);
                prevPos = pos;
            }
            //Debug.DrawLine(transform.position, inputs, Color.blue);
        }
        else if(drawing)
        {
            drawing = false;
            cooldown = 250.0f;

            Vector3 prevPos = transform.position;

            for(int i = 0; i < curQuads; i++)
            {
                float newAngle = quadList[i] * size;
                Quaternion.Euler(0.0f, 0.0f, newAngle);
                Debug.DrawLine(prevPos, transform.position + Quaternion.AngleAxis(newAngle, Vector3.forward) * transform.right * 2.0f, Color.yellow, 1.0f);
                prevPos = transform.position + Quaternion.AngleAxis(newAngle, Vector3.forward) * transform.right * 2.0f;
            }

            quadList[0] = -1;
            quadList[1] = -1;
            quadList[2] = -1;
            curQuads = 0;
            lastQuad = -1;
        }

        //m_weapon.LookAt(m_weapon.position + inputs);
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
        m_weapon.localPosition = Vector3.zero;
        m_weapon.localScale = Vector3.one;
        m_weapon.GetComponent<WeaponPickup>().Swapped();
        m_weapon.rotation = oldRot;
        Destroy(new_weapon.gameObject);
    }

    public void SetPlayerNumber(int set)
    {
        playerNumber = set;
    }

    int GetQuadrant(float angle, float size)
    {
        int start = 3;
        int counter = 0;
        angle += 22.5f;
        if (angle < 0)
        {
            angle += 360.0f;
        }
        if (angle > 360)
        {
            angle -= 360.0f;
        }

        int maxCounter = (int)Mathf.Round(360.0f / size);
        int ret = 0;

        while((counter+1)*size <= 360)
        {
            if(angle >= counter * size && angle < (counter + 1) * size)
            {
                ret = (start - counter);
                if(ret >= maxCounter)
                {
                    ret -= maxCounter;
                }
                if(ret < 0)
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
