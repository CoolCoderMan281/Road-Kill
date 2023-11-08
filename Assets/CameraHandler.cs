using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Camera")]
    public new GameObject camera;
    public Vector3 camera_target_vector3;
    public GameObject camera_target_object;
    public TweenType follow_type;
    public float camera_tween_inbetween;

    public enum TweenType
    {
        LERP, DELTA_TIME
    }

    public void Start()
    {
        // Fool proofing
        DontDestroyOnLoad(this);
        if (camera.GetComponent<Camera>() == null || camera == null)
        {
            Debug.LogError("A camera object has not been selected, or has no camera component!");
        }
        if (camera_tween_inbetween.Equals(0f))
        {
            Debug.Log("No camera tween inbetween was set.. defaulting to 1f!");
            camera_tween_inbetween = 1f;
        }
    }

    public void Update()
    {
        switch(follow_type)
        {
            case TweenType.LERP:
                camera.transform.position = Vector3.Lerp(camera.transform.position, camera_target_vector3, camera_tween_inbetween);
                break;
            case TweenType.DELTA_TIME: // delta time
                if (camera_target_object != null)
                {
                    Vector3 delta = camera_target_object.transform.position - transform.position;
                    transform.position += delta * Time.deltaTime * camera_tween_inbetween;
                }
                break;
            default:
                Debug.LogWarning("Unknown camera follow_type");
                break;
        }
    }
}
