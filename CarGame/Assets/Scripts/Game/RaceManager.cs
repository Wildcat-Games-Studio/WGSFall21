using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;


public class RaceManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;

    public GameObject endPanel;

    public CheckPointManager checkPointManager;
    public CarController carController;
    public PointFollow carFollow;

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

        endPanel.SetActive(false);
        carController.enabled = false;
        carFollow.enabled = false;

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
                        m_last_seconds = m_seconds;
                        carController.enabled = true;
                        UpdateCountDown("GO");

                        m_state = RaceState.Running;
                        break;
                    }

                    if ((int)m_last_seconds != (int)m_seconds)
                    {
                        m_last_seconds = m_seconds;
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
                    
                    if(m_seconds - m_last_seconds >= 0.01f)
                    {
                        m_last_seconds = m_seconds;
                        UpdateTimerText();
                    }
                }
                break;
            case RaceState.Ended:
                {

                }
                break;
        }
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
        m_state = RaceState.Ended;
        carFollow.enabled = true;
        carController.enabled = false;

        SaveTime();

        endPanel.SetActive(true);
    }

    [Serializable]
    struct RaceTime
    {
        public float seconds;
    }

    [Serializable]
    class RaceTimeCollection
    {
        public RaceTime[] Items;
    }

    public void SaveTime()
    {
        List<RaceTime> loadListData;

        string jsonToLoad = Application.persistentDataPath + "/lv1_score.json";
        if(File.Exists(jsonToLoad))
        {
            RaceTimeCollection tmp_collection = JsonUtility.FromJson<RaceTimeCollection>(jsonToLoad);
            RaceTime[] _tempLoadListData = tmp_collection.Items;
            loadListData = _tempLoadListData.ToList();
        }
        else
        {
            loadListData = new List<RaceTime>();
        }

        RaceTime t = new RaceTime();
        t.seconds = m_seconds;

        loadListData.Add(t);

        RaceTimeCollection collection = new RaceTimeCollection();
        collection.Items = loadListData.ToArray();
        string jsonData = JsonUtility.ToJson(collection);

        File.WriteAllText(jsonToLoad, jsonData);
    }
}
