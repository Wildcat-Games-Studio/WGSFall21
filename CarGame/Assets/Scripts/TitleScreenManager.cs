using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject levelScreen;
    public GameObject optionsPanel;

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

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
}
