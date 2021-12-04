using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed;

    public float pitch;
    public float yaw;
    public float distance;
    public Vector3 up = Vector3.up;

    void Start()
    {
        up.Normalize();
    }

    void FixedUpdate()
    {
        // calculate yaw
        Vector3 targetDir = Vector3.ProjectOnPlane(target.forward, up).normalized;

        // create rotation
        Quaternion rotation = Quaternion.AngleAxis(yaw, up) * Quaternion.LookRotation(targetDir, up);
        rotation = Quaternion.AngleAxis(pitch, rotation * Vector3.right) * rotation;
        Vector3 fromTarget = rotation * Vector3.forward;

        // apply new transforms
        transform.rotation = rotation;
        transform.position = Vector3.Lerp(transform.position, target.position - fromTarget * distance, Time.fixedDeltaTime * followSpeed);
    }
}
