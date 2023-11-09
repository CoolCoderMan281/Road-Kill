using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float Increment;

    public Manager manager;

    public void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public void Update()
    {
        Vector3 newPos = transform.position;
        newPos.z -= Increment;
        transform.position = newPos;
        if (newPos.z < -10)
        {
            manager.HandleMiss(gameObject);
        }
    }
}
