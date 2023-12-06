using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player_Obj;
    public GameObject Car_Obj;
    public float MovementIncrement;
    public float Bounds;
    public float Forward_Increase;
    public bool Forward_Held;
    public KeyCode Forward;
    public KeyCode Left;
    public KeyCode Right;
    public bool Debug;
    public bool CanDie;
    public float Size;
    public bool Pause;
    public Vector3 trueSize;
    [Header("Rage")]
    public float RageProgress;
    public float RageIncrement;
    public float RageTick;
    public float RageModifier;
    public float RageHitReward;
    public float RageLoss;
    public bool RageActive;
    public Slider DebugRageProgressSlider;
    public Slider RageMeterSlider;
    public TMP_Text DebugRageProgressLabel;
    public float Original_SpawnedObjectSpeed;
    [Header("Camera Handling")]
    public CameraHandler CameraHandler;
    public GameObject CameraTarget;
    public Coroutine CamCoro = null;
    public float FOV;
    public bool FOVControlled;
    [Header("Spawning")]
    public bool CanSpawn;
    public bool CanSpawnAnimals;
    public bool CanSpawnObstacles;
    public float SpawnIncrement;
    public float SpawnedObjectSpeed;
    public List<Animal_Preset> Animals = new List<Animal_Preset>();
    public List<GameObject> Objects = new List<GameObject>();
    [Header("External")]
    public LevelManager levelManager;
    public MenuHandler menuHandler;
    public Slider Car_Speed_Slider;
    public TMP_Text Car_Speed_Label;

    public void Start()
    {
        GameObject.Find("Debug_Btn").SetActive(Debug);
        GameObject.Find("Debug_notif").SetActive(Debug);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        GameObject MainCamera = GameObject.Find("Main Camera");
        CameraHandler = MainCamera.GetComponent<CameraHandler>();
        CameraHandler.camera_target_object = CameraTarget;
        CameraHandler.camera_tween_inbetween = 5f;
        CameraHandler.follow_type = CameraHandler.TweenType.DELTA_TIME;
        StartCoroutine(SpawnAnimals());
        StartCoroutine(RageMeter());
        StartCoroutine(DifficultyUpdater());
        levelManager = MainCamera.GetComponent<LevelManager>();
        Car_Speed_Slider.value = MovementIncrement;
        Car_Speed_Label.text = "Car Speed ("+Car_Speed_Slider.value.ToString()+")";
        trueSize = Car_Obj.transform.localScale;
        PlayerSizeUpdate();
        FOV = CameraHandler.camera.GetComponent<Camera>().fieldOfView;
    }

    public IEnumerator DifficultyUpdater()
    {
        float savedSpeed = 0;
        while (true)
        {
            if (!Pause && !Forward_Held)
            {
                SpawnedObjectSpeed += 0.01f;
            }
            if (Forward_Held)
            {
                savedSpeed += 0.01f;
            } else
            {
                SpawnedObjectSpeed += savedSpeed;
                savedSpeed = 0;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator ChangeCamFOV(float next)
    {
        float currentFOV = CameraHandler.GetComponent<Camera>().fieldOfView;
        bool increasing = next > currentFOV;
        FOVControlled = true;
        while ((increasing && currentFOV < next) || (!increasing && currentFOV > next))
        {
            currentFOV = Mathf.Lerp(currentFOV, next, 0.025f);
            if (Math.Round(currentFOV) == next)
            {
                CameraHandler.GetComponent<Camera>().fieldOfView = next;
                break;
            }
            CameraHandler.GetComponent<Camera>().fieldOfView = currentFOV;
            yield return null;
        }
        FOVControlled = false;
        CameraHandler.GetComponent<Camera>().fieldOfView = next;
        UnityEngine.Debug.Log("Done");
    }

    public void PlayerSizeUpdate()
    {
        Vector3 scale = trueSize;
        scale.x = Size;
        scale.y = (Size*2);
        scale.z = (Size * 0.8f);
        UnityEngine.Debug.Log("Scale: " + Size);
        UnityEngine.Debug.Log(Size * 0.8f);
        UnityEngine.Debug.Log(scale.x);
        UnityEngine.Debug.Log(scale.y);
        UnityEngine.Debug.Log(scale.z);
        Vector3 pos = Car_Obj.transform.position;
        pos.y = scale.y / 1.5f;
        Car_Obj.transform.position = pos;
        Car_Obj.transform.localScale = scale;
    }

    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        if (menuHandler.currentMenu == menuHandler.GetMenuByName("pause"))
        {
            Pause = true;
        } else
        {
            Pause = false;
        }
        HandleInput(); 
        if (!RageActive && !Pause)
        {
            Original_SpawnedObjectSpeed = SpawnedObjectSpeed;
        }
    }

    // Handle movement speed update
    public void UpdateMovementIncrement()
    {
        MovementIncrement = Car_Speed_Slider.value;
        Car_Speed_Label.text = "Car Speed (" + Car_Speed_Slider.value.ToString() + ")";
    }

    // Handle animal spawning
    public IEnumerator SpawnAnimals()
    {
        while (true)
        {
            if (CanSpawn && !Pause)
            {
                Animal_Preset rand_animal = Animals[UnityEngine.Random.Range(0, Animals.Count)];
                if ((rand_animal.Animal_Type == Animal_Preset.AnimalType.Single && CanSpawnAnimals) || (rand_animal.Animal_Type == Animal_Preset.AnimalType.Obstacle && CanSpawnObstacles))
                {
                    GameObject tmp = Instantiate(rand_animal.Animal_Obj);
                    tmp.transform.position = new Vector3(UnityEngine.Random.Range(-Bounds, Bounds), 0.5f, 255);
                    tmp.AddComponent<Animal>();
                    tmp.GetComponent<Animal>().preset = rand_animal;
                    Objects.Add(tmp);
                    yield return new WaitForSeconds(SpawnIncrement);
                } else
                {
                    if (!(CanSpawnAnimals || CanSpawnObstacles))
                    {
                        yield return new WaitForSeconds(SpawnIncrement);
                    }
                }
            } else
            {
                yield return new WaitForSeconds(SpawnIncrement);
            }
        }
    }

    // Handle rage meter
    public IEnumerator RageMeter()
    {
        while (true)
        {
            if (Pause)
            {
                yield return new WaitForSeconds(RageTick);
            }
            else
            {
                if (RageProgress >= 100 && !RageActive) // Start Rage!
                {
                    RageActive = true;
                    SpawnedObjectSpeed = Original_SpawnedObjectSpeed * RageModifier;
                    StartCoroutine(RageMeterRelease());
                    CamCoro = StartCoroutine(ChangeCamFOV(FOV * 2));
                }
                if (RageProgress == 0 && RageActive) // End Rage!
                {
                    RageActive = false;
                    SpawnedObjectSpeed = Original_SpawnedObjectSpeed;
                    StopCoroutine(RageMeterRelease());
                    CamCoro = StartCoroutine(ChangeCamFOV(FOV));
                }
                DebugRageProgressSlider.value = RageProgress;
                RageMeterSlider.value = RageProgress;
                DebugRageProgressLabel.text = "Rage Progress (" + RageProgress + ")";
                yield return new WaitForSeconds(RageTick);
            }
        }
    }

    // Rage meter release
    public IEnumerator RageMeterRelease()
    {
        while (RageProgress > 0 && RageActive) // Active Rage!
        {
            if (Pause)
            {
                yield return new WaitForSeconds(RageTick);
            }
            RageProgress -= RageLoss;
            yield return new WaitForSeconds(RageTick);
        }
        RageProgress = 0;
    }

    // Handle Input
    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuHandler.currentMenu == menuHandler.GetMenuByName("pause"))
            {
                UiHandler uih = gameObject.AddComponent<UiHandler>();
                uih.action = UiHandler.ActionType.SwitchMenu;
                uih.action_parameter = "MainMenu";
                uih.Click();
            } else {
                UiHandler uih = gameObject.AddComponent<UiHandler>();
                uih.action = UiHandler.ActionType.SwitchMenu;
                uih.action_parameter = "pause";
                uih.Click();
            }
        }
        if (!Pause)
        {
            Forward_Held = Input.GetKey(Forward);
            if (Input.GetKey(Left) || Input.GetKey(KeyCode.LeftArrow))
            {
                // Left
                Vector3 newPosition = Player_Obj.transform.position;
                newPosition.x -= MovementIncrement / 15;
                Player_Obj.transform.position = newPosition;
                // Max bounds
                if (Player_Obj.transform.position.x < -Bounds)

                {
                    Player_Obj.transform.position = new Vector3(-Bounds, 0, 0);
                }
            }
            if (Input.GetKey(Right) || Input.GetKey(KeyCode.RightArrow))
            {
                // Right
                Vector3 newPosition = Player_Obj.transform.position;
                newPosition.x += MovementIncrement / 15;
                Player_Obj.transform.position = newPosition;
                // Max bounds
                if (Player_Obj.transform.position.x > Bounds)
                {
                    Player_Obj.transform.position = new Vector3(Bounds, 0, 0);
                }
            }
        }
    }

    // Hit detection
    public void HandleHit(GameObject obj, Animal_Preset preset)
    {
        if (preset.Animal_Type == Animal_Preset.AnimalType.Single)
        {
            RageProgress += RageIncrement * RageHitReward;
            Objects.Remove(obj);
            Destroy(obj.gameObject);
        } else
        {
            if (CanDie)
            {
                if (CamCoro != null)
                {
                    StopCoroutine(CamCoro);
                }
                CameraHandler.GetComponent<Camera>().fieldOfView = FOV;
                Objects.Remove(obj);
                Destroy(obj.gameObject);
                levelManager.SetLevel(levelManager.GetLevelByName("RoadRage_Lose"));
            }
        }
    }

    // Miss in-point
    public void HandleMiss(GameObject obj, Animal_Preset preset)
    {
        Destroy(obj.gameObject);
    }
}

[Serializable]
public class Animal_Preset
{
    public enum Direction
    {
        None, Left, Right
    }
    public enum AnimalType
    {
        Single, Obstacle
    }

    [Header("Generic")]
    public GameObject Animal_Obj;
    public AnimalType Animal_Type;
    [Header("Movement")]
    public bool Moves;
    public float MovementIncrement;
    public Direction Movement_Direction;

    public Animal_Preset(GameObject animal_Obj, AnimalType animal_Type, bool moves, float movementIncrement, Direction direction, float increment)
    {
        Animal_Obj = animal_Obj;
        Animal_Type = animal_Type;
        Moves = moves;
        MovementIncrement = movementIncrement;
        Movement_Direction = direction;
    }
}