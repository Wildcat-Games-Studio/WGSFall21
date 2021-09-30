using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public Transform target;
    public CheckPoint[] checkPoints;

    private int currentCheckPoint = 0;
    private bool won = false;

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

        setTargetPos();
    }

    /**
     * Test if the checkpoint entered was in the correct order.
     **/
    public void TestPointEntered(CheckPoint chk)
    {
        if (won) return;

        if(chk == checkPoints[currentCheckPoint])
        {
            currentCheckPoint++;

            // This is the end
            if (currentCheckPoint == checkPoints.Length)
            {
                Debug.Log("Finish line crossed");
                won = true;
            }
            else
            {
                setTargetPos();
            }
        }
    }

    void setTargetPos()
    {
        target.position = checkPoints[currentCheckPoint].transform.position;
    }
}
