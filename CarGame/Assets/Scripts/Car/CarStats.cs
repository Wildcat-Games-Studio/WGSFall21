using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/CarStats")]
public class CarStats : ScriptableObject
{
    [Header("Speeds")]
    public float maxSpeed = 50;
    public float maxRevSpeed = 25;
    public float maxTurnSpeed = 2.5f;

    [Header("Accelerations")]
    public float movementAccel = 50;
    public float movementRevAccel = 30;
    public float breakAccel = 100;
    public float torquePower = 50;

    public float wheelCorrectionAccel = 150;
    public float wheelCorrectionDriftAccel = 15;
    public float turnCorrectionAccel = 50;

    public float minTurnSpeed = 1;

    [Header("Suspension")]
    public float targetDistFromGround = 0.5f;
    public float invDampingCoefficient = 0.9f;
    public float springCoefficient = 100.0f;

    public float castRadius = 0.25f;
    public LayerMask castMask;

    public float groundDrag = 0.25f;
    public float groundAngularDrag = 1.3f;

    public Vector3 centerOfMass;
    public Vector3 movementOrigin;

    public float flipForce = 80;
    public Vector3[] suspensionPoints = new Vector3[4];
}
