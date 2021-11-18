using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public TimerManager timerManager;
    public Transform target;

    public bool targetAtFinish = true;

    public CheckPoint[] checkPoints;

    private uint m_currentCheckPoint = 0;

    private void Start()
    {
        // Select checkpoints automaticaly
        if(checkPoints.Length == 0)
        {
            checkPoints = GetComponentsInChildren<CheckPoint>();

            if (checkPoints.Length == 0) Debug.LogError("No children of CheckPointManager have a checkpoint component");
        }

        foreach (CheckPoint p in checkPoints)
        {
            p.setManager(this);
        }

        SetTargetPos();
    }

    /**
     * Test if the checkpoint entered was in the correct order and perform checkpoint logic.
     **/
    public void TestPointEntered(CheckPoint chk)
    {
        if(CompareCheckpoint(chk))
        {
            m_currentCheckPoint++;

            // This is the end
            if (m_currentCheckPoint == checkPoints.Length)
            {
                OnWin();
            }
            else
            {
                SetTargetPos();
            }
        }
    }

    public bool GetLastCheckPoint(ref Vector3 o_pos)
    {
        if (m_currentCheckPoint >= checkPoints.Length || m_currentCheckPoint == 0) return false;

        o_pos = checkPoints[m_currentCheckPoint - 1].transform.position;

        return true;
    }

    /**
     * Perform the required logic when the player crosses the finish line
     **/ 
    void OnWin()
    {
        timerManager.SetCanTick(false);
    }
    
    /**
     * Move the target position to the current checkpoint's location
     **/ 
    void SetTargetPos()
    {
        target.position = checkPoints[m_currentCheckPoint].transform.position;

        if(!targetAtFinish && m_currentCheckPoint == checkPoints.Length - 1)
        {
            target.gameObject.SetActive(false);
        }
    }

    /**
     * Safely compare a given checkpoint to the current checkpoint
     **/ 
    bool CompareCheckpoint(CheckPoint chk)
    {
        if (m_currentCheckPoint >= checkPoints.Length) return false;
        return chk == checkPoints[m_currentCheckPoint];
    }
}
