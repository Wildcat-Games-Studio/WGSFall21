using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager m_manager;
    private AudioSource m_sound;

    private void Start()
    {
        m_sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_sound.Stop();
            m_sound.Play();
            m_manager.OnPointEnter();
        }
    }

    public void setManager(CheckPointManager manager)
    {
        this.m_manager = manager;
    }
}
