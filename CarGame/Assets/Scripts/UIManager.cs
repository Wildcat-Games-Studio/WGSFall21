using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Canvas MainMenuCanvas;
    public Canvas OptionsMenuCanvas;

    public void BeginBtnClicked()
    {
        SceneManager.LoadScene(1);
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
