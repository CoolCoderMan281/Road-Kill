using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player_Obj;
    public float MovementIncrement;
    public float Bounds;
    public KeyCode Left;
    public KeyCode Right;
    [Header("Camera Handling")]
    public CameraHandler CameraHandler;
    public GameObject CameraTarget;
    [Header("Spawning")]
    public float SpawnIncrement;
    public float SpawnedObjectSpeed;

    public void Start()
    {
        GameObject MainCamera = GameObject.Find("Main Camera");
        CameraHandler = MainCamera.GetComponent<CameraHandler>();
        CameraHandler.camera_target_object = CameraTarget;
        CameraHandler.camera_tween_inbetween = 5f;
        CameraHandler.follow_type = CameraHandler.TweenType.DELTA_TIME;
        StartCoroutine(SpawnAnimals());
    }

    public void Update()
    {
        HandleInput();
    }

    // Handle animal spawning
    public IEnumerator SpawnAnimals()
    {
        while (true)
        {
            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tmp.transform.position = new Vector3(Random.Range(-Bounds, Bounds),0.5f,255);
            tmp.AddComponent<Animal>();
            tmp.GetComponent<Animal>().Increment = SpawnedObjectSpeed;
            yield return new WaitForSeconds(SpawnIncrement);
        }
    }

    // Handle Input
    public void HandleInput()
    {
        if (Input.GetKey(Left))
        {
            // Left
            Vector3 newPosition = Player_Obj.transform.position;
            newPosition.x -= MovementIncrement/15;
            Player_Obj.transform.position = newPosition;
            // Max bounds
            if (Player_Obj.transform.position.x < -Bounds)

            {
                Player_Obj.transform.position = new Vector3(-Bounds, 0, 0);
            }
        } else if (Input.GetKey(Right))
        {
            // Right
            Vector3 newPosition = Player_Obj.transform.position;
            newPosition.x += MovementIncrement/15;
            Player_Obj.transform.position = newPosition;
            // Max bounds
            if (Player_Obj.transform.position.x > Bounds)
            {
                Player_Obj.transform.position = new Vector3(Bounds, 0, 0);
            }
        }
    }

    // Hit detection
    public void HandleHit(GameObject obj)
    {
        Debug.Log("Hit");
        Destroy(obj.gameObject);
    }

    // Miss in-point
    public void HandleMiss(GameObject obj)
    {
        Debug.Log("Miss");
        Destroy(obj.gameObject);
    }
}