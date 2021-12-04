using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PointFollow : MonoBehaviour
{
    [Min(0.1f)]
    public float distError = 0.5f;

    public float speed;

    public float turnSlerp;
    public float veloLerp;

    public Transform pointHolder;
    public List<Transform> points;

    public bool subdiv;

    private int m_currentPoint = 0;

    private Vector3 m_lastPos;

    private Rigidbody m_body;

    void Start()
    {
        if (points.Count == 0)
        {
            Debug.LogError("PointFollow points not set.");
        }

        m_body = GetComponent<Rigidbody>();

        m_lastPos = transform.position;
    }

    void FixedUpdate()
    {
        if(InRangeOfTarget())
        {
            m_currentPoint = WrappingAdd(m_currentPoint, 1);
        }

        Vector3 p = points[m_currentPoint].position;

        Ray ray = new Ray(p, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            hit.point = hit.point + hit.normal * 0.2f;

            Debug.DrawRay(hit.point, Vector3.up * 4.0f, Color.cyan);

            Vector3 new_velo = hit.point - transform.position;
            new_velo.y = 0;

            new_velo = new_velo.normalized * speed;

            new_velo.y = m_body.velocity.y;

            m_body.velocity = Vector3.Lerp(m_body.velocity, new_velo, Time.fixedDeltaTime * veloLerp);

            Vector3 targDir = (transform.position - m_lastPos).normalized;
            if (targDir.sqrMagnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targDir, hit.normal), Time.fixedDeltaTime * turnSlerp);
            }
        }
        else
        {
            Debug.Log(points[m_currentPoint] + " is too low");
            m_body.velocity = (p - transform.position);
        }

        m_lastPos = transform.position;
    }

    bool InRangeOfTarget()
    {
        float sqrDst = (points[m_currentPoint].position - transform.position).sqrMagnitude;
        return sqrDst < distError * distError;
    }

    int WrappingAdd(int v, int r)
    {
        return (v + r) % points.Count;
    }

    private Vector3 Bezier(Vector3 v0, Vector3 v1, Vector3 v2, float t)
    {
        t = Mathf.Clamp01(t);

        float omt = 1 - t;

        return omt * omt * v0 + 2 * omt * t * v1 + t * t * v2;
    }

    private void OnDrawGizmosSelected()
    {
        if (points == null) return;

        List<Vector3> subdivPoints = new List<Vector3>();

        for(int i = 0; i < points.Count; i++)
        {
            Vector3 v0 = points[i].position;
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(v0, distError);

            subdivPoints.Add(v0);

            if (points.Count % 2 == 0 && points.Count > 3 && subdiv)
            {
                Vector3 v1 = points[WrappingAdd(i, 1)].position;
                Vector3 v2 = points[WrappingAdd(i, 2)].position;
                Gizmos.color = Color.green;

                for(float f = 0.1f; f < 1.0; f += 0.1f)
                {
                    Vector3 p = Bezier(v0, v1, v2, f);
                    subdivPoints.Add(p);
                    Gizmos.DrawSphere(p, distError);
                }

                i += 1;
            }

        }

        if(pointHolder != null)
        {
            points.Clear();

            foreach (Transform t in pointHolder)
            {
                DestroyImmediate(t.gameObject);
            }

            int i = 0;
            foreach(Vector3 v in subdivPoints)
            {
                GameObject go = new GameObject();
                go.name = string.Format("Point ({0})", i);
                go.transform.position = v;
                go.transform.SetParent(pointHolder);

                points.Add(go.transform);

                i++;
            }

            Debug.Log("Points have been calculated and put into point holder");
            pointHolder = null;
        }


    }
}
