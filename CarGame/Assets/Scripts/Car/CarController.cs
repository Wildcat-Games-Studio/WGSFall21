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

    public CarStats stats;
    public CheckPointManager checkPointManager;
    public CarSuspension suspension;
    public CarSoundManager soundManager;

    private Rigidbody body;

    private Vector2 inputMovement;
    private bool drifting;
    private bool flipping;

    private Vector3 m_startPos;
    private Quaternion m_startRot;

    public void Pause(bool pause)
    {
    }
    private void Start()
    {
        m_startPos = transform.position;
        m_startRot = transform.rotation;

        body = GetComponent<Rigidbody>();

        body.centerOfMass = stats.centerOfMass;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Respawn"))
        {
            Vector3 respawnPos = new Vector3();
            Quaternion respawnRot = new Quaternion();
            if(checkPointManager.GetLastCheckPoint(ref respawnPos, ref respawnRot))
            {
                transform.position = respawnPos + Vector3.up * 1.5f;
                transform.rotation = respawnRot;
            }
            else
            {
                transform.position = m_startPos;
                transform.rotation = m_startRot;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGrounding(suspension.WheelsOnGround > 0);
        if (flipping && suspension.WheelsOnGround == 0)
        {
            body.AddTorque(transform.forward * stats.flipForce);
            //body.AddForceAtPosition(Vector3.up * stats.flipForce, transform.TransformPoint(body.centerOfMass + Vector3.right));
        }

        if (suspension.WheelsOnGround == 0) return;

        // Movement
        Vector3 forwardDir = Vector3.ProjectOnPlane(transform.forward, suspension.GroundNormal).normalized;
        float relativeSpeed = Vector3.Dot(body.velocity, forwardDir);
        float relativeDir = Mathf.Sign(relativeSpeed);

        soundManager.setWindVolume(Mathf.Abs(relativeSpeed));

        Vector2 inputRaw = new Vector2(Mathf.Sign(inputMovement.x), Mathf.Sign(inputMovement.y));

        float relMaxSpeed = relativeDir > 0 ? stats.maxSpeed : stats.maxRevSpeed;

        if (Mathf.Abs(relativeSpeed) < relMaxSpeed)
        {
            /** Moved in here because there's no need for it unless relativespeed < relmaxspeed */
            float movementForce = CalculateMovementForce(relativeDir, inputRaw);
            // Clamp speed
            body.AddForceAtPosition(forwardDir * movementForce * suspension.WheelsOnGround, transform.TransformPoint(stats.movementOrigin));
        }

        // Turning
        CalculateTurning(relativeSpeed, relativeDir, inputRaw);
        // Keep the car moving in direction of its wheels
        AlignVehicle(suspension.GroundNormal, forwardDir, relativeDir);
    }

    void CheckGrounding(bool isGrounded)
    {
        if (isGrounded)
        {
            body.drag = stats.groundDrag;
            body.angularDrag = stats.groundAngularDrag;
        }
        else
        {
            body.drag = 0.0f;
            body.angularDrag = 0.05f;
        }

    }

    void CalculateTurning(float inRelativeSpeed, float inRelativeDirection, Vector2 inInputRaw)
    {
        float turnTorque = inputMovement.x * stats.torquePower;
        if (!Mathf.Approximately(inputMovement.x, 0) && Mathf.Abs(inRelativeSpeed) > stats.minTurnSpeed)
        {
            if (Vector3.Dot(body.angularVelocity, inRelativeDirection * inInputRaw.x * transform.up) < stats.maxTurnSpeed)
            {
                body.AddTorque(transform.up * turnTorque * inRelativeDirection * suspension.WheelsOnGround);
            }
        }
        else
        {
            body.AddTorque(transform.up * -Vector3.Dot(transform.up, body.angularVelocity) * stats.turnCorrectionAccel * suspension.WheelsOnGround);
        }
    }


    float CalculateMovementForce(float inRelativeDir, Vector2 inInputRaw)
    {
        // use correct acceleration based on difference between movement direction and input direction
        if (inRelativeDir != inInputRaw.y)
        {
            return inputMovement.y * stats.breakAccel;
        }
        else if (inRelativeDir > 0) // backwards
        {
            return inputMovement.y * stats.movementAccel;
        }
        else
        {
            return inputMovement.y * stats.movementRevAccel;
        }
    }

    void AlignVehicle(Vector3 normal, Vector3 forwardDir, float relativeDir)
    {
        Vector3 veloProj = Vector3.ProjectOnPlane(body.velocity, normal);

        Vector3 toForward = Vector3.Project(body.velocity, forwardDir.normalized) - veloProj;

        /**
        Debug.DrawRay(worldPoint, Vector3.Project(body.velocity, forwardDir.normalized), Color.green);
        Debug.DrawRay(worldPoint, veloProj, Color.cyan);
        Debug.DrawRay(worldPoint + veloProj, toForward, Color.red);
        */
        if (drifting && relativeDir > 0)
        {
            body.AddForce(toForward * stats.wheelCorrectionDriftAccel * suspension.WheelsOnGround);
        }
        else
        {
            body.AddForce(toForward * stats.wheelCorrectionAccel * suspension.WheelsOnGround);
        }
    }

    void WheelDebug(Vector3 worldPoint, Vector3 forwardDir, Vector3 veloProj, Vector3 toForward)
    {
        Debug.DrawRay(worldPoint, Vector3.Project(body.velocity, forwardDir.normalized), Color.green);
        Debug.DrawRay(worldPoint, veloProj, Color.cyan);
        Debug.DrawRay(worldPoint + veloProj, toForward, Color.red);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.TransformPoint(stats.centerOfMass), 0.05f);
        Gizmos.DrawSphere(transform.TransformPoint(stats.movementOrigin), 0.05f);
    }
}
