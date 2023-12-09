using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float Increment;
    public float LastIncrement;
    public Animal_Preset preset;
    public Manager manager;
    public Vector3 target;

    public void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        StartCoroutine(Walking());
    }

    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        if (!manager.Pause && !manager.ImpactActive)
        {
            Increment = manager.SpawnedObjectSpeed;
            if(manager.Forward_Held)
            {
                Vector3 tmp1 = new Vector3(LastIncrement, 0, 0);
                Vector3 tmp2 = new Vector3(Increment + manager.Forward_Increase, 0, 0);
                Increment = Vector3.Lerp(tmp1, tmp2, 0.05f).x;
            } else
            {
                Vector3 tmp1 = new Vector3(LastIncrement, 0, 0);
                Vector3 tmp2 = new Vector3(Increment, 0, 0);
                Increment = Vector3.Lerp(tmp1, tmp2, 0.05f).x;
            }
            LastIncrement = Increment;
            Vector3 newPos = transform.position;
            newPos.z -= Increment;
            newPos.y = 0.5f;
            transform.position = newPos;
            if (newPos.z < -10)
            {
                manager.HandleMiss(gameObject, preset);
            }
            target.y = newPos.y;
            target.z = newPos.z;
            newPos = Vector3.Lerp(newPos, target, 0.005f);
            transform.position = newPos;
        }
    }

    public IEnumerator Walking()
    {
        while (true)
        {
            if (manager.Pause)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
            Vector3 newPos = transform.position;
            if (preset.Moves)
            {
                if (preset.Movement_Direction == Animal_Preset.Direction.Left)
                {
                    if (newPos.x > -manager.Bounds)
                    {
                        newPos.x -= preset.MovementIncrement;
                    }
                    else
                    {
                        preset.Movement_Direction = Animal_Preset.Direction.Right;
                    }
                }
                else
                {
                    if (newPos.x < manager.Bounds)
                    {
                        newPos.x += preset.MovementIncrement;
                    }
                    else
                    {
                        preset.Movement_Direction = Animal_Preset.Direction.Left;
                    }
                }
            }
            target = newPos;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
