using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    [Header("Action")]
    public MenuHandler menuHandler;
    public DialogueHandler dialougeHandler;
    public CameraHandler cameraHandler;
    public LevelManager levelManager;
    public AudioHandler audioHandler;
    public ActionType action;
    public string action_parameter;
    public TMP_Text action_label;

    public enum ActionType { SwitchMenu, SwitchLevel, CloseDialogue, StartDialogue, SetCameraTarget, Mute_SFX, Mute_MUSIC, Music_Volume, SFX_Volume, MainMenu, FPS_DISPLAY, UpdateSpeed, 
                             UpdateCamY, UpdateCamZ, UpdateCollisionVisibility, UpdateAnimalSpeed, UpdateRageIncrement, UpdateRageTick, UpdateRageProgress, UpdateRageSpeed,
                             UpdateHitRageReward, UpdateCanDie, UpdateCanSpawn, UpdateCanSpawnAnimal, UpdateCanSpawnObstacle, UpdateSpawnIncrement, KillObjects,
                             ObjectSelector, ObjectType, UpdateCamFOV, UpdateCamXRot, UpdateSize, UpdateBoostBuff, UpdateRageLoss, UpdateDiffTick, UpdateImpactTime }

    public void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    public void Start()
    {
        try {
            menuHandler = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();
            dialougeHandler = GameObject.Find("Main Camera").GetComponent<DialogueHandler>();
            cameraHandler = GameObject.Find("Main Camera").GetComponent<CameraHandler>();
            levelManager = GameObject.Find("Main Camera").GetComponent<LevelManager>();
            audioHandler = GameObject.Find("Main Camera").GetComponent<AudioHandler>();
        }
        catch
        {
            Debug.LogWarning("I'm having issues defining scripts! from "+gameObject.name);
        }

        switch(action)
        {
            case ActionType.UpdateCamY:
                GameObject cam_poser = GameObject.Find("Camera_Positioner");
                GameObject.Find("CamY_Label").GetComponent<TMP_Text>().text = "CamY (" + cam_poser.transform.position.y + ")";
                gameObject.GetComponent<Slider>().value = cam_poser.transform.position.y;
                break;
            case ActionType.UpdateCamZ:
                cam_poser = GameObject.Find("Camera_Positioner");
                GameObject.Find("CamZ_Label").GetComponent<TMP_Text>().text = "CamZ (" + cam_poser.transform.position.z + ")";
                gameObject.GetComponent<Slider>().value = cam_poser.transform.position.z;
                break;
            case ActionType.UpdateCollisionVisibility:
                MeshRenderer mr = GameObject.Find("Player_Obj").GetComponent<MeshRenderer>();
                gameObject.GetComponent<Toggle>().isOn = mr.enabled;
                break;
            case ActionType.UpdateAnimalSpeed:
                Slider self = gameObject.GetComponent<Slider>();
                Manager mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.SpawnedObjectSpeed;
                TMP_Text label = GameObject.Find("Animal_Speed_Label").GetComponent<TMP_Text>();
                label.text = "Animal Speed (" + mgr.SpawnedObjectSpeed + ")";
                break;
            case ActionType.UpdateRageIncrement:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageIncrement;
                label = GameObject.Find("Rage_Increment_Label").GetComponent<TMP_Text>();
                label.text = "Rage Increment (" + mgr.RageIncrement + ")";
                break;
            case ActionType.UpdateRageTick:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageTick;
                label = GameObject.Find("Rage_Tick_Label").GetComponent<TMP_Text>();
                label.text = "Rage Tick (" + mgr.RageTick + ")";
                break;
            case ActionType.UpdateRageProgress:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageProgress;
                action_label.text = "Rage Progress (" + mgr.RageProgress + ")";
                break;
            case ActionType.UpdateRageSpeed:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageModifier;
                label = GameObject.Find("Rage_Speed_Label").GetComponent<TMP_Text>();
                label.text = "Rage Speed (" + mgr.RageModifier + ")";
                break;
            case ActionType.UpdateHitRageReward:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageHitReward;
                action_label.text = "Rage Reward (" + mgr.RageHitReward + ")";
                break;
            case ActionType.UpdateCanDie:
                Toggle toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                toggle_self.isOn = mgr.CanDie;
                break;
            case ActionType.UpdateCanSpawn:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                toggle_self.isOn = mgr.CanSpawn;
                break;
            case ActionType.UpdateCanSpawnAnimal:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                toggle_self.isOn = mgr.CanSpawnAnimals;
                break;
            case ActionType.UpdateCanSpawnObstacle:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                toggle_self.isOn = mgr.CanSpawnObstacles;
                break;
            case ActionType.UpdateSpawnIncrement:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.SpawnIncrement;
                action_label.text = "Spawn tick (" + mgr.SpawnIncrement + ")";
                break;
            case ActionType.ObjectSelector:
                TMP_Dropdown dropdown_self = gameObject.GetComponent<TMP_Dropdown>();
                dropdown_self.ClearOptions();
                // Create dropdown data
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                List<TMP_Dropdown.OptionData> drop = new List<TMP_Dropdown.OptionData>();
                foreach(Animal_Preset preset in mgr.Animals)
                {
                    TMP_Dropdown.OptionData dropdown_option = new TMP_Dropdown.OptionData();
                    dropdown_option.text = preset.Animal_Obj.name;
                    drop.Add(dropdown_option);
                }
                Debug.Log(drop.Count.ToString());
                dropdown_self.AddOptions(drop.ToList());
                break;
            case ActionType.ObjectType:
                dropdown_self = gameObject.GetComponent<TMP_Dropdown>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                string target = GameObject.Find("ObjectSelector").GetComponent<TMP_Dropdown>().itemText.text;
                Debug.Log(target);
                foreach(Animal_Preset animal_Preset in mgr.Animals)
                {
                    if (target == animal_Preset.Animal_Obj.name)
                    {
                        if (animal_Preset.Animal_Type == Animal_Preset.AnimalType.Single)
                        {
                            dropdown_self.value = 0;
                        } else
                        {
                            dropdown_self.value = 1;
                        }
                    }
                }
                break;
            case ActionType.UpdateCamFOV:
                self = gameObject.GetComponent<Slider>();
                self.value = cameraHandler.camera.GetComponent<Camera>().fieldOfView;
                action_label.text = "CamFOV (" + self.value + ")";
                break;
            case ActionType.UpdateCamXRot:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.XRot;
                action_label.text = "CamX Rot (" + self.value + ")";
                break;
            case ActionType.UpdateSize:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.Size;
                action_label.text = "Car Scale (" + self.value + ")";
                break;
            case ActionType.UpdateBoostBuff:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.Forward_Increase;
                action_label.text = "Boost buff (" + self.value + ")";
                break;
            case ActionType.UpdateRageLoss:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.RageLoss;
                action_label.text = "Rage Loss (" + self.value + ")";
                break;
            case ActionType.UpdateDiffTick:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.DiffTick;
                action_label.text = "Diff Tick (" + self.value + ")";
                break;
            case ActionType.UpdateImpactTime:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                self.value = mgr.ImpactTime;
                action_label.text = "Impact Time (" + self.value + ")";
                break;
        }
    }

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            switch(action)
            {
                case ActionType.UpdateAnimalSpeed:
                    Slider self = gameObject.GetComponent<Slider>();
                    Manager mgr = GameObject.Find("Manager").GetComponent<Manager>();
                    self.value = mgr.SpawnedObjectSpeed;
                    TMP_Text label = GameObject.Find("Animal_Speed_Label").GetComponent<TMP_Text>();
                    label.text = "Animal Speed (" + mgr.SpawnedObjectSpeed + ")";
                    break;
                case ActionType.UpdateCamFOV:
                    self = gameObject.GetComponent<Slider>();
                    mgr = GameObject.Find("Manager").GetComponent<Manager>();
                    self.value = cameraHandler.camera.GetComponent<Camera>().fieldOfView;
                    self.interactable = !mgr.FOVControlled;
                    action_label.text = "CamFOV (" + self.value + ")";
                    break;
            }
        }   
    }

    public void Click()
    {
        if (menuHandler == null)
        {
            menuHandler = GameObject.Find("MenuHandler").GetComponent<MenuHandler>();
        }
        switch(action)
        {
            case ActionType.SwitchMenu:
                menuHandler.SetCurrentMenu(menuHandler.GetMenuByName(action_parameter));
                break;
            case ActionType.CloseDialogue:
                dialougeHandler.Hide();
                break;
            case ActionType.StartDialogue:
                dialougeHandler.SetDialouge(dialougeHandler.GetDialogueByName(action_parameter));
                break;
            case ActionType.SetCameraTarget:
                GameObject target = GameObject.Find(action_parameter);
                cameraHandler.camera_target_object = target;
                break;
            case ActionType.SwitchLevel:
                levelManager.SetLevel(levelManager.GetLevelByName(action_parameter));
                break;
            case ActionType.Mute_SFX:
                audioHandler.SupressSFX = GetComponentInParent<Toggle>().isOn;
                audioHandler.UpdateAudios();
                break;
            case ActionType.Mute_MUSIC:
                audioHandler.SupressMUSIC = GetComponentInParent<Toggle>().isOn;
                audioHandler.UpdateAudios();
                break;
            case ActionType.Music_Volume:
                audioHandler.MUSIC_Volume = GetComponentInParent<UnityEngine.UI.Slider>().value;
                audioHandler.UpdateAudios();
                break;
            case ActionType.SFX_Volume:
                audioHandler.SFX_Volume = GetComponentInParent<UnityEngine.UI.Slider>().value;
                audioHandler.UpdateAudios();
                break;
            case ActionType.MainMenu:
                levelManager.SetLevel(levelManager.MainMenu_Level);
                break;
            case ActionType.FPS_DISPLAY:
                cameraHandler.fps_overlay_active = !cameraHandler.fps_overlay_active;
                if (!cameraHandler.fps_overlay_active)
                {
                    cameraHandler.fps_overlay.text = "";
                }
                break;
            case ActionType.UpdateSpeed:
                Manager mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.UpdateMovementIncrement();
                break;
            case ActionType.UpdateCamY:
                GameObject cam_poser = GameObject.Find("Camera_Positioner");
                Vector3 newPos = cam_poser.transform.position; newPos.y = gameObject.GetComponent<Slider>().value;
                cam_poser.transform.position = newPos;
                GameObject.Find("CamY_Label").GetComponent<TMP_Text>().text = "CamY (" + cam_poser.transform.position.y + ")";
                break;
            case ActionType.UpdateCamZ:
                cam_poser = GameObject.Find("Camera_Positioner");
                newPos = cam_poser.transform.position; newPos.z = gameObject.GetComponent<Slider>().value;
                cam_poser.transform.position = newPos;
                GameObject.Find("CamZ_Label").GetComponent<TMP_Text>().text = "CamZ (" + cam_poser.transform.position.z + ")";
                break;
            case ActionType.UpdateCollisionVisibility:
                GameObject player_obj = GameObject.Find("Player_Obj");
                MeshRenderer mr = player_obj.GetComponent<MeshRenderer>();
                mr.enabled = !mr.enabled;
                gameObject.GetComponent<Toggle>().isOn = mr.enabled;
                break;
            case ActionType.UpdateAnimalSpeed:
                Slider self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.SpawnedObjectSpeed = self.value;
                TMP_Text label = GameObject.Find("Animal_Speed_Label").GetComponent<TMP_Text>();
                label.text = "Animal Speed (" + mgr.SpawnedObjectSpeed + ")";
                break;
            case ActionType.UpdateRageIncrement:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageIncrement = self.value;
                label = GameObject.Find("Rage_Increment_Label").GetComponent<TMP_Text>();
                label.text = "Rage Increment (" + mgr.RageIncrement + ")";
                break;
            case ActionType.UpdateRageTick:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageTick = self.value;
                label = GameObject.Find("Rage_Tick_Label").GetComponent<TMP_Text>();
                label.text = "Rage Tick (" + mgr.RageTick + ")";
                break;
            case ActionType.UpdateRageProgress:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageProgress = self.value;
                action_label.text = "Rage Progress (" + mgr.RageProgress + ")";
                break;
            case ActionType.UpdateRageSpeed:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageModifier = self.value;
                label = GameObject.Find("Rage_Speed_Label").GetComponent<TMP_Text>();
                label.text = "Rage Speed (" + mgr.RageModifier + ")";
                break;
            case ActionType.UpdateHitRageReward:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageHitReward = self.value;
                action_label.text = "Rage Reward (" + mgr.RageHitReward + ")";
                break;
            case ActionType.UpdateCanDie:
                Toggle toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.CanDie = toggle_self.isOn;
                break;
            case ActionType.UpdateCanSpawn:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.CanSpawn = toggle_self.isOn;
                break;
            case ActionType.UpdateCanSpawnAnimal:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.CanSpawnAnimals = toggle_self.isOn;
                break;
            case ActionType.UpdateCanSpawnObstacle:
                toggle_self = gameObject.GetComponent<Toggle>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.CanSpawnObstacles = toggle_self.isOn;
                break;
            case ActionType.UpdateSpawnIncrement:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.SpawnIncrement = self.value;
                action_label.text = "Spawn tick (" + mgr.SpawnIncrement + ")";
                break;
            case ActionType.KillObjects:
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                List<GameObject> list = new List<GameObject>();
                list = mgr.Objects; // Working object list
                foreach (GameObject obj in list.ToList())
                {
                    try
                    {
                        mgr.Objects.Remove(obj);
                        Destroy(obj);
                    }
                    catch { }
                }
                break;
            case ActionType.ObjectSelector:
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                TMP_Dropdown dropdown_self = gameObject.GetComponent<TMP_Dropdown>();
                GameObject tmp_dr = GameObject.Find("ObjectType");
                tmp_dr.GetComponent<UiHandler>().Click();
                
                break;
            case ActionType.ObjectType:
                dropdown_self = gameObject.GetComponent<TMP_Dropdown>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                string dropdown_target = GameObject.Find("ObjectSelector").GetComponent<TMP_Dropdown>().itemText.text;
                foreach (Animal_Preset animal_Preset in mgr.Animals)
                {
                    if (dropdown_target == animal_Preset.Animal_Obj.name)
                    {
                        if (animal_Preset.Animal_Type == Animal_Preset.AnimalType.Single)
                        {
                            dropdown_self.value = 0;
                        }
                        else
                        {
                            dropdown_self.value = 1;
                        }
                    }
                }
                break;
            case ActionType.UpdateCamFOV:
                self = gameObject.GetComponent<Slider>();
                cameraHandler.camera.GetComponent<Camera>().fieldOfView = self.value;
                action_label.text = "CamFOV (" + self.value + ")";
                break;
            case ActionType.UpdateCamXRot:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.XRot = self.value;
                action_label.text = "CamX Rot (" + self.value + ")";
                break;
            case ActionType.UpdateSize:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.Size = self.value;
                action_label.text = "Car Scale (" + self.value + ")";
                mgr.PlayerSizeUpdate();
                break;
            case ActionType.UpdateBoostBuff:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.Forward_Increase = self.value;
                action_label.text = "Boost buff (" + self.value + ")";
                break;
            case ActionType.UpdateRageLoss:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.RageLoss = self.value;
                action_label.text = "Rage Loss (" + self.value + ")";
                break;
            case ActionType.UpdateDiffTick:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.DiffTick = self.value;
                action_label.text = "Diff Tick (" + self.value + ")";
                break;
            case ActionType.UpdateImpactTime:
                self = gameObject.GetComponent<Slider>();
                mgr = GameObject.Find("Manager").GetComponent<Manager>();
                mgr.ImpactTime = self.value;
                action_label.text = "Impact Time (" + self.value + ")";
                break;
        }
    }
}
