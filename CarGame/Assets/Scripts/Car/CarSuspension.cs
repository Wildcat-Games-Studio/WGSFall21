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
            Debug.DrawRay(worldPoint, transform.forward, Color.green);
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
            Vector3 worldPoint = transform.TransformPoint(stats.suspensionPoints[i]) + transform.up * stats.castRadius;
            Ray ray = new Ray(worldPoint, -transform.up);
            RaycastHit hit;

            m_wheelNormals[i] = transform.up;

            if (Physics.SphereCast(ray, stats.castRadius, out hit, stats.targetDistFromGround + stats.castRadius, stats.castMask))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) < 0.3f)
                {
                    continue;
                }

                WheelsOnGround++;
                GroundNormal += hit.normal;
                m_wheelDistances[i] = Mathf.Clamp(hit.distance, stats.castRadius, stats.targetDistFromGround);
                m_wheelNormals[i] = hit.normal;

                // Suspension
                float x = 1.0f - hit.distance / stats.targetDistFromGround;

                body.AddForceAtPosition(transform.up * (stats.invDampingCoefficient * stats.springCoefficient * x), worldPoint);
            }
            else
            {
                m_wheelDistances[i] = stats.targetDistFromGround * (Vector3.Dot(transform.up, Vector3.up) * 0.5f + 0.5f);
            }
        }

        GroundNormal /= WheelsOnGround;
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < stats.suspensionPoints.Length; i++)
        {
            Vector3 drawPoint = transform.TransformPoint(stats.suspensionPoints[i]);

            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(drawPoint, stats.castRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(drawPoint, -transform.up * stats.targetDistFromGround);
        }
    }
}
