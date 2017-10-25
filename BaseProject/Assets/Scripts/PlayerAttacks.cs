using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour {

    public Transform m_weapon;
    int playerNumber = 1;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        Vector2 inputs;
        inputs.x = -Input.GetAxis("AttackHorz" + playerNumber);
        inputs.y = -Input.GetAxis("AttackVert" + playerNumber);

        if (inputs.sqrMagnitude > 1)
        {
            m_weapon.eulerAngles = new Vector3(m_weapon.eulerAngles.x, m_weapon.eulerAngles.y, Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg);
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
}
