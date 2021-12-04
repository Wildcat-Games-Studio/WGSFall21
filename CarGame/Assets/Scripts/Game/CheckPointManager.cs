using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public CheckPoint target;

    public bool targetAtFinish = true;

    public delegate void OnWin();
    public OnWin winFunc;

    public Transform[] checkPoints;
    private uint m_currentCheckPoint = 0;

    private bool m_parentedPoints;

    private void Start()
    {
        // Select checkpoints automaticaly
        if(checkPoints.Length == 0)
        {
            checkPoints = GetComponentsInChildren<Transform>(false);
            if (checkPoints.Length == 0) Debug.LogError("No children of CheckPointManager and checkpoints not set.");
            m_currentCheckPoint = 1;
            m_parentedPoints = true;
        }
        else
        {
            m_parentedPoints = false;
        }

        target.setManager(this);
        SetTargetPos();
    }

    /**
     * Test if the checkpoint entered was in the correct order and perform checkpoint logic.
     **/
    public void OnPointEnter()
    {
        if(m_currentCheckPoint < checkPoints.Length)
        {
            m_currentCheckPoint++;

            // This is the end
            if (m_currentCheckPoint == checkPoints.Length)
            {
                winFunc?.Invoke();
                target.gameObject.SetActive(false);
            }
            else
            {
                SetTargetPos();
            }
        }
    }

    public bool GetLastCheckPoint(ref Vector3 o_pos)
    {
        if (m_currentCheckPoint >= checkPoints.Length || m_currentCheckPoint == 0 ||
            m_parentedPoints && m_currentCheckPoint == 1) return false;

        o_pos = checkPoints[m_currentCheckPoint - 1].transform.position;

        return true;
    }
    
    /**
     * Move the target position to the current checkpoint's location
     **/ 
    void SetTargetPos()
    {
        target.transform.position = checkPoints[m_currentCheckPoint].position;

        //if(!targetAtFinish && m_currentCheckPoint == checkPoints.Length - 1)
        //{
        //    target.gameObject.SetActive(false);
        //}
    }
}
