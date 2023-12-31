using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public List<Level> levels = new List<Level>();
    [Header("Special Levels")]
    public Level Init_Level;
    public Level MainMenu_Level;
    [Header("Internal")]
    public Level ActiveLevel;

    // Startup & Persistence
    public void Start()
    {
        DontDestroyOnLoad(this);
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName(Init_Level.sceneName)) { Debug.LogError("Did not start in init scene, expect broken functionality!"); }
        SetLevel(MainMenu_Level);
    }

    // Tick activelevel update
    public void Update() { if (ActiveLevel != null && ActiveLevel.update != null) { ActiveLevel.update(); } }

    // Set the ActiveLevel
    public void SetLevel(Level NextLevel)
    {
        if (NextLevel == null)
        {
            Debug.LogError("A null level has been passed, please pass a real level");
        }
        if (ActiveLevel != null && ActiveLevel != NextLevel)
        {
            //if (ActiveLevel.end != null) { ActiveLevel.end(); }
            Debug.Log("Ended level " + ActiveLevel.name);
            if (NextLevel == null)
            {
                NextLevel = MainMenu_Level;
            }
            ActiveLevel = NextLevel;
            //if (ActiveLevel.start != null) { ActiveLevel.start(); }
            try
            {
                SceneManager.LoadScene(ActiveLevel.sceneName);
            } catch
            {
                Debug.Log("Failed!");
            }
            Debug.Log("Started level " + ActiveLevel.name);
            if (ActiveLevel.autoAction == Level.AutoAction.CameraReset)
            {
                GameObject.Find("Main Camera").transform.position = new Vector3(0, 0, 0);
            }
        }
        else
        {
            Debug.Log("ActiveLevel was unset, or that level is already selected.");
        }
    }

    public void MainMenu()
    {
        SetLevel(MainMenu_Level);
    }

    public Level GetLevelByName(string name)
    {
        foreach(Level lvl in levels)
        {
            if (lvl.name == name)
            {
                return lvl;
            }
        }
        return null;
    }

    // End the ActiveLevel (SetLevel, without a new follow-up level)
    public void EndLevel()
    {
        if (ActiveLevel != null)
        {
            if (ActiveLevel.end != null) { ActiveLevel.end(); }
            Debug.Log("Ended level " + ActiveLevel.name);
            ActiveLevel = null;
        }
        else
        {
            Debug.Log("No level is active.");
        }
    }
}

[System.Serializable]
public class Level
{
    public enum AutoAction { CameraReset }

    public string name;
    public int id;
    public string sceneName;
    public string affiliation;
    public AutoAction autoAction;
    public Func<string> start;
    public Func<string> update;
    public Func<string> end;
    public Level(string name, int id, string sceneName, AutoAction autoAction, string affiliation = null, Func<string> start=null, Func<string> update=null, Func<string> end=null)
    {
        this.name = name;
        this.id = id;
        this.sceneName = sceneName;
        this.affiliation = affiliation;
        this.autoAction = autoAction;
        this.start = start;
        this.update = update;
        this.end = end;
        Debug.Log("New level " + name + " defined!");
    }
}