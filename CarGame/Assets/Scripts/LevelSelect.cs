using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{

    public static void LoadLevel(int LevelIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(LevelIndex);
    }
}
