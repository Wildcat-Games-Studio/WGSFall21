using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            manager.TestPointEntered(this);
        }
    }

    public void setManager(CheckPointManager manager)
    {
        this.manager = manager;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1.0f);
        Gizmos.DrawRay(transform.position, transform.forward * 2.0f);
    }

}
