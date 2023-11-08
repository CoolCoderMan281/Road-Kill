using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueHandler : MonoBehaviour
{
    [Header("Dialogue")]
    public List<Dialogue> dialogs = new List<Dialogue>();
    [Header("Internal")]
    public Dialogue ActiveDialogue;
    public GameObject DialogueMenu;
    public GameObject DialogueMenuClose;
    public UnityEngine.UI.Image DialogueImage;
    public Sprite NoSprite;
    public TMP_Text DialogueMenuText;
    public GameObject Canvas;

    public void Start()
    {
        DontDestroyOnLoad(this);
        Hide();
    }

    public void SetDialouge(Dialogue NextDialogue)
    {
        try { Canvas = GameObject.Find("Canvas"); Canvas.SetActive(false); }
        catch { Debug.LogWarning("No canvas found in the scene!"); }
        if (ActiveDialogue != null && ActiveDialogue != NextDialogue)
        {
            if (ActiveDialogue.end != null) { ActiveDialogue.end(); }
            Hide(true);
            ActiveDialogue = NextDialogue;
            DialogueMenu.SetActive(true);
            if (ActiveDialogue.sprite == null)
            {
                ActiveDialogue.sprite = NoSprite;
            }
            DialogueMenuText.text = ActiveDialogue.text;
            DialogueImage.sprite = ActiveDialogue.sprite;
            DialogueMenuClose.SetActive(true);
            if (ActiveDialogue.start != null) { ActiveDialogue.start(); }
            Debug.Log("Started dialogue " + ActiveDialogue.name);
        } else if (ActiveDialogue == null)
        {
            ActiveDialogue = NextDialogue;
            DialogueMenu.SetActive(true);
            if (ActiveDialogue.sprite == null)
            {
                ActiveDialogue.sprite = NoSprite;
            }
            DialogueMenuText.text = ActiveDialogue.text;
            DialogueImage.sprite = ActiveDialogue.sprite;
            DialogueMenuClose.SetActive(true);
            if (ActiveDialogue.start != null) { ActiveDialogue.start(); }
            Debug.Log("Started dialogue " + ActiveDialogue.name);
        }
        else
        {
            Debug.Log("ActiveDialogue was unset, or that dialogue is already selected.");
        }
    }

    public Dialogue GetDialogueByName(string name)
    {
        foreach (Dialogue dialouge_i in dialogs)
        {
            if (dialouge_i.name == name)
            {
                return dialouge_i;
            }
        }
        Debug.LogError("Did not find dialogue by the name " + name);
        return null;
    }

    public void Hide(bool showCanvas=false)
    {
        DialogueMenuText.text = "";
        DialogueMenu.SetActive(false);
        DialogueMenuClose.SetActive(false);
        DialogueImage.sprite = null;
        Debug.Log("Ended dialogue " + ActiveDialogue.name);
        ActiveDialogue = null;
        if (!showCanvas)
        {
            try { Canvas.SetActive(true); Canvas = null; }
            catch { Debug.LogWarning("No canvas found in the scene!"); }
        }
    }
}

[System.Serializable]
public class Dialogue
{
    public string name;
    public int id;
    public string text;
    public string affiliation;
    public List<Option> options;
    public Sprite sprite;
    public Func<string> start;
    public Func<string> update;
    public Func<string> end;
    public Dialogue(string name, int id, string text, Sprite sprite, List<Option> options, string affiliation = null, Func<string> start = null, Func<string> update = null, Func<string> end = null)
    {
        this.name = name;
        this.id = id;
        this.text = text;
        this.options = options;
        this.affiliation = affiliation;
        this.sprite = sprite;
        this.start = start;
        this.update = update;
        this.end = end;
        Debug.Log("New dialouge " + name + " defined!");
    }
}

[Serializable]
public class Option
{
    public string text;
    public string next_dialogue;
}