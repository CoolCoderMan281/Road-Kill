using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    [Header("Camera")]
    public new GameObject camera;
    public Vector3 camera_target_vector3;
    public GameObject camera_target_object;
    public TweenType follow_type;
    public float camera_tween_inbetween;
    [Header("External")]
    public GameObject MenuHandler;
    public AudioHandler ah;
    public Coroutine CurrentMusic;
    public TMP_Text fps_overlay;
    public bool fps_overlay_active = false;
    public float timer, refresh, avgFramerate;
    public Texture2D last_loss;

    public enum TweenType
    {
        LERP, DELTA_TIME
    }

    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    public void Start()
    {
        //Screen.SetResolution(1920, 1080, true);
        // Fool proofing
        ah = GetComponent<AudioHandler>();
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;
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
        if (fps_overlay_active)
        {
            float timelapse = Time.smoothDeltaTime;
            timer = timer <= 0 ? refresh : timer -= timelapse;

            if (timer <= 0) avgFramerate = (int)(1f / timelapse);
            fps_overlay.text = avgFramerate.ToString() + " FPS";
        }
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

    public void OnLevelWasLoaded(int level)
    {
        UpdateMusic(level);
    }

    public void UpdateMusic(int level)
    {
        GameObject display = GameObject.Find("Lose");
        if (display != null)
        {
            display.GetComponent<RawImage>().texture = last_loss;
            Debug.Log("Set texture!");
            ah.PlayMusic(ah.GetAudioByName("Title"));
        }
        if (level == 2)
        {
            ah.PlayMusic(ah.GetAudioByName("RushHour"));
        }
        if (level == 1)
        {
            if (ah.currentMusic != ah.GetAudioByName("Title"))
            {
                ah.PlayMusic(ah.GetAudioByName("Title"));
            }
        }
    }
}
