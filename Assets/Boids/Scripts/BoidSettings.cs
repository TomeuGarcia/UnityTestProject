using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoidSettings
{
    [SerializeField, Range(0.0f, 30.0f)] public float maxSpeed = 5.0f;
    [SerializeField, Range(0.0f, 30.0f)] public float maxAcceleration = 5.0f;
    [SerializeField, Range(0.0f, 30.0f)] public float maxForce = 5.0f;

    [SerializeField, Range(0.0f, 180.0f)] public float sightAngle = 90.0f;
    [SerializeField, Range(0.0f, 20.0f)] public float sightDistance = 2.0f;
    public float sightThreshold { get; private set; }

    [SerializeField, Range(0.0f, 1.0f)] public float separationWeight = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] public float alignmentWeight = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] public float cohesionWeight = 1.0f;

    [Header("Obstacles")]
    [SerializeField, Range(0.0f, 10.0f)] public float obstacleAvoidanceWeight = 6.0f;
    [SerializeField, Range(0.01f, 3.0f)] public float spherecastRadius = 0.1f;
    public LayerMask obstacleProveMask;

    [Header("Target")]
    [SerializeField] public Transform target;
    [SerializeField, Range(0.0f, 10.0f)] public float targetWeight = 0.2f;
    public bool TargetExists => target != null;
    public Vector3 TargetPosition => target.position;

    public void Update()
    {
        sightThreshold = Mathf.Cos(Mathf.Deg2Rad * sightAngle);

        float totalWeight = separationWeight + alignmentWeight + cohesionWeight;
        separationWeight /= totalWeight;
        alignmentWeight /= totalWeight;
        cohesionWeight /= totalWeight;
    }
}