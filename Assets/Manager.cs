using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player_Obj;
    public float MovementIncrement;
    public KeyCode Left;
    public KeyCode Right;
    [Header("Camera Handling")]
    public CameraHandler CameraHandler;
    public GameObject CameraTarget;

    public void Start()
    {
        GameObject MainCamera = GameObject.Find("Main Camera");
        CameraHandler = MainCamera.GetComponent<CameraHandler>();
        CameraHandler.camera_target_object = CameraTarget;
        CameraHandler.camera_tween_inbetween = 5f;
        CameraHandler.follow_type = CameraHandler.TweenType.DELTA_TIME;
    }

    public void Update()
    {
        HandleInput();
    }

    // Handle Input
    public void HandleInput()
    {
        if (Input.GetKeyDown(Left))
        {
            // Left
            Vector3 newPosition = Player_Obj.transform.position;
            newPosition.x -= MovementIncrement;
            Player_Obj.transform.position = newPosition;
        } else if (Input.GetKeyDown(Right))
        {
            // Right
            Vector3 newPosition = Player_Obj.transform.position;
            newPosition.x += MovementIncrement;
            Player_Obj.transform.position = newPosition;
        }
    }

    // Handle Camera
    public void HandleCamera()
    {
        // Make camera face the object
    }
}