using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundManager : MonoBehaviour
{
    public AudioSource windSource;
    public AudioSource engineSource;
    public float maxWindSpeed;
    public float minEngineSound;

    public void setWindVolume(float speed)
    {
        windSource.volume = speed / maxWindSpeed;
        engineSource.volume = Mathf.Max(speed / maxWindSpeed, minEngineSound);
    }
}
