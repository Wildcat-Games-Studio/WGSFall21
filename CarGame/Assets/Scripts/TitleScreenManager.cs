using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject LevelScreen;

    public void SelectLevelBtnClicked()
    {
        if (mainScreen == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (LevelScreen == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        mainScreen.SetActive(false);
        LevelScreen.SetActive(true);
    }

    public void ToMainMenuBtnClicked()
    {
        if (mainScreen == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (LevelScreen == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        mainScreen.SetActive(true);
        LevelScreen.SetActive(false);
    }

    public void ToggleOptionsMenu()
    {
        
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
}
