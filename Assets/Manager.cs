using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player_Obj;
    public float MovementIncrement;
    public float Bounds;
    public KeyCode Left;
    public KeyCode Right;
    public bool Debug;
    public bool CanDie;
    [Header("Rage")]
    public float RageProgress;
    public float RageIncrement;
    public float RageTick;
    public float RageModifier;
    public float RageHitReward;
    public bool RageActive;
    public Slider DebugRageProgressSlider;
    public Slider RageMeterSlider;
    public TMP_Text DebugRageProgressLabel;
    public float Original_SpawnedObjectSpeed;
    [Header("Camera Handling")]
    public CameraHandler CameraHandler;
    public GameObject CameraTarget;
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
    public int CameraFOV = 55;

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
        Camera.main.fieldOfView = CameraFOV;
        StartCoroutine(SpawnAnimals());
        StartCoroutine(RageMeter());
        levelManager = MainCamera.GetComponent<LevelManager>();
        Car_Speed_Slider.value = MovementIncrement;
        Car_Speed_Label.text = "Car Speed ("+Car_Speed_Slider.value.ToString()+")";
        
    }

    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    public void Update()
    {
        HandleInput();
        if (!RageActive)
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
            if (CanSpawn)
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
            if (RageProgress >= 100 && !RageActive) // Start Rage!
            {
                RageActive = true;
                SpawnedObjectSpeed = Original_SpawnedObjectSpeed * RageModifier;
                StartCoroutine(RageMeterRelease());

                CameraFOV = 90;
                Camera.main.fieldOfView = CameraFOV;
            }
            if (RageProgress == 0 && RageActive) // End Rage!
            {

                RageActive = false;
                SpawnedObjectSpeed = Original_SpawnedObjectSpeed;
                StopCoroutine(RageMeterRelease());

                CameraFOV = 55;
                Camera.main.fieldOfView = CameraFOV;
            }
            if (RageProgress < 100 && !RageActive) // Increment Rage!
            {
                RageProgress += RageIncrement;
            }
            DebugRageProgressSlider.value = RageProgress;
            RageMeterSlider.value = RageProgress;
            DebugRageProgressLabel.text = "Rage Progress (" + RageProgress + ")";
            yield return new WaitForSeconds(RageTick);
        }
    }

    // Rage meter release
    public IEnumerator RageMeterRelease()
    {
        while (RageProgress > 0 && RageActive) // Active Rage!
        {
            RageProgress -= RageIncrement*2;
            if (RageProgress % 3 == 0)
            {
                Camera.main.fieldOfView = CameraFOV--;
            }
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