using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Transform point4;

    void Start ()
    {
        Instantiate(player1, point1.position, point1.rotation);
        Instantiate(player2, point2.position, point2.rotation);
        Instantiate(player3, point3.position, point3.rotation);
        Instantiate(player4, point4.position, point4.rotation);
    }
}
