using System.Collections;
using System.Collections.Generic;
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

    public enum ActionType { SwitchMenu, SwitchLevel, CloseDialogue, StartDialogue, SetCameraTarget, Mute_SFX, Mute_MUSIC, Music_Volume, SFX_Volume, MainMenu, FPS_DISPLAY, UpdateSpeed}

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
        }
    }
}
