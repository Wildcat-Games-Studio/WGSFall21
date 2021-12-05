using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public LayerMask raycastMask;

    public Transform target;
    public float followSpeed;
    public float trackSpeed;

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
        Vector3 fromTarget = rotation * Vector3.back;

        float dist = distance;

        Ray ray = new Ray(target.position, fromTarget);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10);
        if(Physics.SphereCast(ray, 0.3f, out hit, dist, raycastMask))
        {
            dist = hit.distance;
        }

        // apply new transforms
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * trackSpeed);
        transform.position = Vector3.Lerp(transform.position, target.position + fromTarget * dist, Time.fixedDeltaTime * followSpeed);
    }
}
