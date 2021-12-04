using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarSuspension : MonoBehaviour
{
    public CarStats stats;

    public Transform[] wheelTransforms;

    public int WheelsOnGround { get; private set; }
    public Vector3 GroundNormal { get; private set; }

    private float[] m_wheelDistances;
    private Vector3[] m_wheelNormals;

    private void LateUpdate()
    {
        for (int i = 0; i < wheelTransforms.Length; i++)
        {
            // recalc 
            Vector3 worldPoint = transform.TransformPoint(stats.suspensionPoints[i]);
            // Wheel Placement
            wheelTransforms[i].position = worldPoint - transform.up * m_wheelDistances[i];
            // Wheel Rotation
            Vector3 wheelFacing = transform.forward;

            wheelTransforms[i].rotation = Quaternion.LookRotation(wheelFacing, m_wheelNormals[i]);
        }
    }

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        m_wheelDistances = new float[wheelTransforms.Length];
        m_wheelNormals = new Vector3[wheelTransforms.Length];
}

    private void FixedUpdate()
    {
        WheelsOnGround = 0;
        GroundNormal = Vector3.zero;

        for (int i = 0; i < stats.suspensionPoints.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(stats.suspensionPoints[i]);
            Ray ray = new Ray(worldPoint, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, stats.targetDistFromGround))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);

                WheelsOnGround++;
                GroundNormal += hit.normal;

                if(i < m_wheelDistances.Length)
                {
                    m_wheelDistances[i] = stats.targetDistFromGround * (Vector3.Dot(transform.up, Vector3.up) * 0.5f + 0.5f);
                    m_wheelNormals[i] = transform.up;
                }

                // Suspension
                float x = 1.0f - hit.distance / stats.targetDistFromGround;

                body.AddForceAtPosition(transform.up * (stats.invDampingCoefficient * stats.springCoefficient * x), worldPoint);
            }
        }

        GroundNormal /= WheelsOnGround;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < stats.suspensionPoints.Length; i++)
        {
            Vector3 drawPoint = transform.TransformPoint(stats.suspensionPoints[i]);

            Gizmos.DrawRay(drawPoint, -transform.up * stats.targetDistFromGround);
            Gizmos.DrawSphere(drawPoint, 0.05f);
        }
    }
}
