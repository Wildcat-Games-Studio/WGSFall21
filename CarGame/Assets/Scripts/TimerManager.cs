using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    float m_milliSeconds;
    uint m_seconds;
    uint m_minutes;
    bool m_isActive = false;

    public void SetCanTick(bool active)
    {
        m_isActive = active;
    }

    void Start()
    {
        m_milliSeconds = 0.0f;
        m_seconds = 0;
        m_minutes = 0;
    }

    void Update()
    {
        if(m_isActive)
        {
            Tick();
        }
    }


    void Tick()
    {
        m_milliSeconds += Time.deltaTime;

        if (m_milliSeconds * 100.0f >= 1.0f) // check if 1/100 has changed: ie. 0.01 <- last digit here
        {
            UpdateTime();
            UpdateText();
        }
    }

    void UpdateTime()
    {
        if (m_milliSeconds >= 1.0f)
        {
            m_milliSeconds -= 1.0f;

            m_seconds += 1;

            if (m_seconds >= 60)
            {
                m_seconds -= 60;
                m_minutes += 1;
            }
        }
    }

    void UpdateText()
    {
        timerText.text = string.Format("{0}'{1}\"{2:D2}", m_minutes, m_seconds, (uint)(m_milliSeconds * 100));
    }
}
