using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [Header("Menus")]
    public List<Menu> menuList = new List<Menu>();
    public Menu mainMenu;
    public Menu currentMenu;

    public void Start()
    {
        DontDestroyOnLoad(this);
        if (mainMenu == null) { Debug.LogError("No main menu has been defined!"); }
        menuList.Add(mainMenu);
        SetCurrentMenu(mainMenu);
    }

    public void SetCurrentMenu(Menu menu)
    {
        if (currentMenu == mainMenu && !menu.allowedInMainMenu)
        {
            Debug.LogWarning("Can't open this menu inside of the main menu!");
            return;
        }
        foreach (Menu menu_i in menuList)
        {
            menu_i.menuObject.SetActive(false);
        }
        menu.menuObject.SetActive(true);
    }

    public Menu GetMenuByName(string name)
    {
        foreach (Menu menu_i in menuList)
        {
            if (menu_i.name == name)
            {
                return menu_i;
            }
        }
        Debug.LogError("Did not find menu by the name " + name);
        return null;
    }
}
[System.Serializable]
public class Menu
{
    public GameObject menuObject;
    public string name;
    public bool allowedInPause;
    public bool allowedInMainMenu;

    public Menu(GameObject menuObject, string name, bool allowedInPause, bool allowedInMainMenu)
    {
        this.menuObject = menuObject;
        this.name = name;
        this.allowedInPause = allowedInPause;
        this.allowedInMainMenu = allowedInMainMenu;
    }

    public bool visible()
    {
        return menuObject.activeInHierarchy;
    }
}