using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public TimerManager timerManager;
    public CarController carController;

    public GameObject readyText;
    public GameObject goText;

    private float m_countdown = 0;

    private void Update()
    {

        if (m_countdown >= 3.0f)
        {
            timerManager.SetCanTick(true);
            carController.SetCanMove(true);
            readyText.SetActive(false);
            goText.SetActive(true);

            Destroy(goText, 1.0f);
            // Don't care :'(
            Destroy(this);
        }
        else
        {
            m_countdown += Time.deltaTime;
        }
    }
}
