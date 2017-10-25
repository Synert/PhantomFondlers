using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject projectilePrefab;

    private List<GameObject> Projectiles = new List<GameObject>();
    private List<GameObject> Projectiles2 = new List<GameObject>();

    private float projectileVelocityRight;
    private float projectileVelocityLeft;

    // Use this for initialization
    void Start ()
    {
        projectileVelocityRight = 5;
        projectileVelocityLeft = 5;
    }

    // Update is called once per frame
    void Update ()
    {
		if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet.transform.Translate(new Vector3(0.5f, 0));
            Projectiles.Add(bullet);
        }

        for (int i = 0; i < Projectiles.Count; i++)
        {
            GameObject goBullet = Projectiles[i];
            if (goBullet != null)
            {
                goBullet.transform.Translate(new Vector3(1,0) * Time.deltaTime * projectileVelocityRight);

                Vector3 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position);
                if (bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet);
                    Projectiles.Remove(goBullet);
                }
                
            }
        }

        
        if (Input.GetButtonDown("Fire2"))
        {
            GameObject bullet2 = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            bullet2.transform.Translate(new Vector3(-0.5f, 0));
            Projectiles2.Add(bullet2);
        }

        for (int i = 0; i < Projectiles2.Count; i++)
        {
            GameObject goBullet2 = Projectiles2[i];
            if (goBullet2 != null)
            {
                goBullet2.transform.Translate(new Vector3(-1, 0) * Time.deltaTime * projectileVelocityLeft);

                Vector3 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet2.transform.position);
                if (bulletScreenPos.x >= Screen.width || bulletScreenPos.x <= 0)
                {
                    DestroyObject(goBullet2);
                    Projectiles2.Remove(goBullet2);
                }

            }
        }
        
    }
}
