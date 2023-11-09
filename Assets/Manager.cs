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
}