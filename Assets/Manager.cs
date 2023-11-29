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
    [Header("Camera Handling")]
    public CameraHandler CameraHandler;
    public GameObject CameraTarget;
    [Header("Spawning")]
    public float SpawnIncrement;
    public float SpawnedObjectSpeed;
    public List<Animal_Preset> Animals = new List<Animal_Preset>();
    [Header("External")]
    public LevelManager levelManager;
    public MenuHandler menuHandler;
    public Slider Car_Speed_Slider;
    public TMP_Text Car_Speed_Label;

    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        GameObject MainCamera = GameObject.Find("Main Camera");
        CameraHandler = MainCamera.GetComponent<CameraHandler>();
        CameraHandler.camera_target_object = CameraTarget;
        CameraHandler.camera_tween_inbetween = 5f;
        CameraHandler.follow_type = CameraHandler.TweenType.DELTA_TIME;
        StartCoroutine(SpawnAnimals());
        levelManager = MainCamera.GetComponent<LevelManager>();
        Car_Speed_Slider.value = MovementIncrement;
        Car_Speed_Label.text = "Car Speed ("+Car_Speed_Slider.value.ToString()+")";
    }

    public void Update()
    {
        HandleInput();
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
            
            Animal_Preset rand_animal = Animals[UnityEngine.Random.Range(0, Animals.Count)];
            GameObject tmp = Instantiate(rand_animal.Animal_Obj);
            tmp.transform.position = new Vector3(UnityEngine.Random.Range(-Bounds, Bounds),0.5f,255);
            tmp.AddComponent<Animal>();
            tmp.GetComponent<Animal>().preset = rand_animal;
            yield return new WaitForSeconds(SpawnIncrement);
        }
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
            Destroy(obj.gameObject);
        } else
        {
            Destroy(obj.gameObject);
            levelManager.SetLevel(levelManager.GetLevelByName("RoadRage_Lose"));
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
        Single, Herd
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