using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;


public class RaceManager : MonoBehaviour
{
    public string levelName;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public Animator countdownAnimator;

    public GameObject optionsPanel;

    public GameObject endPanel;
    public TextMeshProUGUI endTimeText;
    public TextMeshProUGUI[] leaderBoard;

    public CheckPointManager checkPointManager;
    public CarController carController;
    public PointFollow carFollow;

    public GameObject lapTeleporter;

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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
            {
                optionsPanel.SetActive(false);
                unpauseGame();
            }
            else
            {
                optionsPanel.SetActive(true);
                pauseGame();
                return;
            }
        }

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
                        countdownText.color = Color.green;

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

    private void pauseGame()
    {
        Time.timeScale = 0;
        countdownAnimator.speed = 0;
    }

    private void unpauseGame()
    {
        Time.timeScale = 1;
        countdownAnimator.speed = 1;
    }

    void UpdateCountDown(int value)
    {
        countdownText.text = string.Format("{0}", value);
    }

    void UpdateCountDown(string text)
    {
        countdownText.text = string.Format(text);
    }

    string FormatSecondsToTime(float in_seconds)
    {
        int minutes = (int)(in_seconds / 60.0f);
        int seconds = (int)in_seconds % 60;
        int milli = (int)((in_seconds - (int)in_seconds) * 100.0f);

        return string.Format("{0}'{1:D2}\"{2:D2}", minutes, seconds, milli);
    }

    void UpdateTimerText()
    {
        timerText.text = FormatSecondsToTime(m_seconds);
    }

    private void OnWin()
    {
        if(lapTeleporter != null)
        {
            lapTeleporter.SetActive(false);
        }

        m_state = RaceState.Ended;
        carFollow.enabled = true;
        carController.enabled = false;

        timerText.text = string.Empty;
        endTimeText.text = FormatSecondsToTime(m_seconds);

        int slot = SaveTime();

        List<RaceTime> scoreBoard = LoadTimes();
        int iter = Math.Min(scoreBoard.Count, leaderBoard.Length);
        for(int i = 0; i < iter; i++)
        {
            leaderBoard[i].text = FormatSecondsToTime(scoreBoard[i].seconds);
            if(i == slot)
            {
                leaderBoard[i].color = Color.green;
            }
        }

        endPanel.SetActive(true);
    }

    [Serializable]
    class RaceTime : IComparable
    {
        public float seconds;

        public int CompareTo(object obj)
        {
            RaceTime otherTemperature = obj as RaceTime;
            if (otherTemperature != null)
                return this.seconds.CompareTo(otherTemperature.seconds);
            else
                throw new ArgumentException("Object is not a RaceTime");
        }
    }

    [Serializable]
    class RaceTimeCollection
    {
        public RaceTime[] Items;
    }

    private List<RaceTime> LoadTimes()
    {
        List<RaceTime> loadListData;

        string path = Application.persistentDataPath + "/" + levelName + "_score.json";
        if (File.Exists(path))
        {
            string jsonToLoad = File.ReadAllText(path);
            RaceTimeCollection tmp_collection = JsonUtility.FromJson<RaceTimeCollection>(jsonToLoad);
            RaceTime[] _tempLoadListData = tmp_collection.Items;
            loadListData = _tempLoadListData.ToList();
        }
        else
        {
            loadListData = new List<RaceTime>();
        }

        return loadListData;
    }

    /** return slot that time was saved to, or -1 if value was not saved **/
    private int SaveTime()
    {
        string path = Application.persistentDataPath + "/" + levelName + "_score.json";
        List<RaceTime> loadListData = LoadTimes();

        RaceTime t = new RaceTime();
        t.seconds = m_seconds;

        loadListData.Add(t);

        loadListData.Sort();

        if(loadListData.Count >= 6)
        {
            loadListData.RemoveRange(6, loadListData.Count - 6);
        }

        int res = -1;

        for(int i = 0; i < loadListData.Count; i++)
        {
            if(Mathf.Approximately(loadListData[i].seconds, t.seconds))
            {
                res = i;
                break;
            }
        }

        RaceTimeCollection collection = new RaceTimeCollection();
        collection.Items = loadListData.ToArray();
        string jsonData = JsonUtility.ToJson(collection);

        File.WriteAllText(path, jsonData);

        return res;
    }
}
