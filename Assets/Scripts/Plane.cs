using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plane : MonoBehaviour
{
    [HideInInspector] public bool onGround = false;
    public float speed = 10f;
    public float airportShift = 0f;
    [HideInInspector] public TerrainGenerator tg;
    [HideInInspector] public float terrainLength;

    void Start()
    {
        if (!onGround)
            GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void Update()
    {
        if (transform.position.x >= terrainLength || transform.position.z >= terrainLength || transform.position.x < 0f || transform.position.z < 0f)
        {
            tg.GeneratePlanes(1);
            Destroy(gameObject);
        }
    }
}
