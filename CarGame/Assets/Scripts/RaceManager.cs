using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public CheckPointManager checkPointManager;
    public CarController carController;

    [Min(0)]
    public int countdownTime;

    private bool countdownClear;

    // Time
    private float m_seconds;
    private float m_last_seconds;

    enum RaceState { CountDown, Running, Ended }
    private RaceState m_state;

    private void Start()
    {
        m_state = RaceState.CountDown;
        m_seconds = 0.0f;
        m_last_seconds = 0.0f;
        countdownClear = false;

        checkPointManager.winFunc = OnWin;

        UpdateCountDown(countdownTime);
    }

    private void Update()
    {
        m_seconds += Time.deltaTime;

        switch (m_state)
        {
            case RaceState.CountDown:
                {
                    if(m_seconds > countdownTime)
                    {
                        m_seconds = 0.0f;
                        carController.SetCanMove(true);
                        UpdateCountDown("GO");

                        m_state = RaceState.Running;
                        break;
                    }

                    if ((int)m_last_seconds != (int)m_seconds)
                    {
                        UpdateCountDown(countdownTime - (int)m_seconds);
                    }

                }break;
            case RaceState.Running:
                {
                    if(!countdownClear && m_seconds > 2.0f)
                    {
                        countdownClear = true;
                        UpdateCountDown("");
                    }

                    UpdateTimerText();
                }
                break;
            case RaceState.Ended:
                {

                }
                break;
        }

        m_last_seconds = m_seconds;
    }

    void UpdateCountDown(int value)
    {
        countdownText.text = string.Format("{0}", value);
    }

    void UpdateCountDown(string text)
    {
        countdownText.text = string.Format(text);
    }

    void UpdateTimerText()
    {
        int minutes = (int)(m_seconds / 60.0f);
        int seconds = (int)m_seconds % 60;
        int milli = (int)((m_seconds - (int)m_seconds) * 100.0f);

        timerText.text = string.Format("{0}'{1:D2}\"{2:D2}", minutes, seconds, milli);
    }

    private void OnWin()
    {
        Debug.Log("Won!!");
    }
}
