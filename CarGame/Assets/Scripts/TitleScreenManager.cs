using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject levelScreen;
    public GameObject optionsPanel;
    public GameObject clearTimePanel;

    public void ToggleLevelSelectBtn()
    {
        if (mainScreen == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (levelScreen == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        mainScreen.SetActive(!mainScreen.activeSelf);
        levelScreen.SetActive(!levelScreen.activeSelf);
    }

    public void ToggleClearTimeBtn()
    {
        if (mainScreen == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (clearTimePanel == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        mainScreen.SetActive(!mainScreen.activeSelf);
        clearTimePanel.SetActive(!clearTimePanel.activeSelf);
    }

    public void ToggleOptionsMenu()
    {
        if (mainScreen == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (optionsPanel == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        mainScreen.SetActive(!mainScreen.activeSelf);
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void ClearDataButtonClicked()
    {
        string path = Application.persistentDataPath + "/Scores";
        if(Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
}
