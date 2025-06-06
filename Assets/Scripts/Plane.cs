using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plane : MonoBehaviour
{
    [HideInInspector] public bool onGround = false;
    public float speed = 10f;
    public float airportShift = 0f;

    void Start()
    {
        if (!onGround)
            GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
