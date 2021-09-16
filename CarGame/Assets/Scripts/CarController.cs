using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController: MonoBehaviour
{
    // RIGIDBODY
    // Mass         10
    // Drag         0.2
    // Angular Drag 0.3

    public float maxSpeed = 50;
    public float maxRevSpeed = 25;
    public float maxTurnSpeed = 2.5f;

    public float movementAccel = 50;
    public float movementRevAccel = 30;
    public float breakAccel = 100;
    public float torquePower = 50;

    public float wheelCorrectionAccel = 150;
    public float wheelCorrectionDriftAccel = 15;
    public float turnCorrectionAccel = 50;

    public float minTurnSpeed = 1;

    public float targetDistFromGround = 0.5f;
    public float invDampingCoefficient = 0.9f;
    public float springCoefficient = 100.0f;

    public float groundDrag = 0.25f;
    public float groundAngularDrag = 1.3f;

    public Vector3 centerOfMass;
    public Vector3 movementOrigin;

    public float flipForce = 80;

    public Vector3[] suspensionPoints = new Vector3[4];

    public Transform[] wheelTransforms = new Transform[4];

    private Rigidbody body;

    private float[] wheelDistances = new float[4];
    private Vector3[] wheelNormals = new Vector3[4];
    private Vector3 wheelDirection;

    private Vector2 inputMovement;
    private bool drifting;
    private bool flipping;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        body.centerOfMass = centerOfMass;
    }

    private void Update()
    {
        // Get input per frame
        inputMovement.x = Input.GetAxis("Horizontal");
        inputMovement.y = Input.GetAxis("Vertical");

        //TODO: Change to axis for controller support
        drifting = Input.GetKey(KeyCode.LeftShift);
        flipping = Input.GetKey(KeyCode.Space);
    }

    private void LateUpdate()
    {
        for(int i = 0; i < 4; i++)
        {
            // recalc 
            Vector3 worldPoint = transform.TransformPoint(suspensionPoints[i]);
            // Wheel Placement
            wheelTransforms[i].position = worldPoint - transform.up * wheelDistances[i];
            // Wheel Rotation
            Vector3 wheelFacing = transform.forward;

            if(i < 2)
            {
                wheelFacing = Quaternion.AngleAxis(-maxTurnSpeed * inputMovement.x * Mathf.Rad2Deg, transform.up) * wheelFacing;
            }

            wheelTransforms[i].rotation = Quaternion.LookRotation(wheelFacing, wheelNormals[i]);
        }
    }

    private void FixedUpdate()
    {
        // spring: F = -kx

        float x = 0.0f;
        RaycastHit hit;
        Ray ray;
        bool grounded = false;

        wheelDirection = Vector3.zero;

        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            wheelDistances[i] = targetDistFromGround * (Vector3.Dot(transform.up, Vector3.up) * 0.5f + 0.5f);
            wheelNormals[i] = transform.up;

            Vector3 worldPoint = transform.TransformPoint(suspensionPoints[i]);
            ray = new Ray(worldPoint, -transform.up);
            if (Physics.Raycast(ray, out hit, targetDistFromGround))
            {
                grounded = true;

                wheelDistances[i] = hit.distance;
                wheelNormals[i] = hit.normal;

                // Suspension
                x = 1.0f - hit.distance / targetDistFromGround;

                body.AddForceAtPosition(transform.up * (invDampingCoefficient * springCoefficient * x), worldPoint);

                // Movement
                Vector3 forwardDir = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                float relativeSpeed = Vector3.Dot(body.velocity, forwardDir);
                float relativeDir = Mathf.Sign(relativeSpeed);

                Vector2 inputRaw = new Vector2(Mathf.Sign(inputMovement.x), Mathf.Sign(inputMovement.y));

                float movementForce = inputMovement.y;

                // use correct acceleration based on difference between movement direction and input direction
                if(relativeDir != inputRaw.y)
                {
                    movementForce *= breakAccel;
                }
                else if(relativeDir > 0) // backwards
                {
                    movementForce *= movementAccel;
                }
                else
                {
                    movementForce *= movementRevAccel;
                }

                Debug.DrawRay(worldPoint, forwardDir);

                float relMaxSpeed = relativeDir > 0 ? maxSpeed : maxRevSpeed;

                if (Mathf.Abs(relativeSpeed) < relMaxSpeed)
                {
                    // Clamp speed
                    body.AddForceAtPosition(forwardDir * movementForce, transform.TransformPoint(movementOrigin));
                }

                // Turning
                float turnTorque = inputMovement.x * torquePower;
                if(!Mathf.Approximately(inputMovement.x, 0) && Mathf.Abs(relativeSpeed) > minTurnSpeed)
                {
                    if (Vector3.Dot(body.angularVelocity, relativeDir * inputRaw.x * transform.up) < maxTurnSpeed)
                    {
                        body.AddTorque(transform.up * turnTorque * relativeDir);
                    }
                }
                else
                {
                    body.AddTorque(transform.up * -Vector3.Dot(transform.up, body.angularVelocity) * turnCorrectionAccel);
                }

                // Keep the car moving in direction of its wheels
                Vector3 veloProj = Vector3.ProjectOnPlane(body.velocity, hit.normal);

                Vector3 toForward = Vector3.Project(body.velocity, forwardDir.normalized) - veloProj;

                Debug.DrawRay(worldPoint, Vector3.Project(body.velocity, forwardDir.normalized), Color.green);
                Debug.DrawRay(worldPoint, veloProj, Color.cyan);
                Debug.DrawRay(worldPoint + veloProj, toForward, Color.red);

                if (drifting && relativeDir > 0)
                {
                    body.AddForce(toForward * wheelCorrectionDriftAccel);
                }
                else
                {
                    body.AddForce(toForward * wheelCorrectionAccel);
                }

                // average directions
                wheelDirection += forwardDir;
            }

        }

        if (grounded)
        {
            body.drag = groundDrag;
            body.angularDrag = groundAngularDrag;
        }
        else
        {
            body.drag = 0.0f;
            body.angularDrag = 0.05f;
        }

        wheelDirection /= 4.0f;

        if(flipping && Vector3.Dot(transform.up, Vector3.down) > 0.8f) //TODO: Check on the ground
        {
            body.AddForceAtPosition(Vector3.up * flipForce, transform.TransformPoint(body.centerOfMass + Vector3.right));
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for(int i = 0; i < suspensionPoints.Length; i++)
        {
            Vector3 drawPoint = transform.TransformPoint(suspensionPoints[i]);

            Gizmos.DrawRay(drawPoint, -transform.up * targetDistFromGround);
            Gizmos.DrawSphere(drawPoint, 0.05f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.TransformPoint(centerOfMass), 0.05f);
        Gizmos.DrawSphere(transform.TransformPoint(movementOrigin), 0.05f);
    }
}
