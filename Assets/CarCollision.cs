using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLISION!!");
        other.gameObject.GetComponent<Animal>().manager.HandleHit(other.gameObject,other.gameObject.GetComponent<Animal>().preset);
    }
}
