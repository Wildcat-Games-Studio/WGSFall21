using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager m_manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_manager.OnPointEnter();
        }
    }

    public void setManager(CheckPointManager manager)
    {
        this.m_manager = manager;
    }
}
