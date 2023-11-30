using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Animal>().manager.HandleHit(other.gameObject,other.gameObject.GetComponent<Animal>().preset);
    }
}
