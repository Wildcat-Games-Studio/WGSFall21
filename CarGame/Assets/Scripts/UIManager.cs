using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas MainMenuCanvas;
    public Canvas OptionsMenuCanvas;
    public Canvas LevelSelectionCanvas;

    public void SelectLevelBtnClicked()
    {
        if (MainMenuCanvas == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (LevelSelectionCanvas == null)
        {
            Debug.Log("LevelSelectionCanvas is NULL. Cannot display.");
            return;
        }

        MainMenuCanvas.enabled = !MainMenuCanvas.enabled;
        LevelSelectionCanvas.enabled = !LevelSelectionCanvas.enabled;
    }

    public void ToggleOptionsMenu()
    {
        if (MainMenuCanvas == null)
        {
            Debug.Log("MainMenuCanvas is NULL. Cannot disable.");
            return;
        }
        if (OptionsMenuCanvas == null)
        {
            Debug.Log("OptionsMenuCanvas is NULL. Cannot display.");
            return;
        }

        MainMenuCanvas.enabled = !MainMenuCanvas.enabled;
        OptionsMenuCanvas.enabled = !OptionsMenuCanvas.enabled;
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
}
