using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidSpawner boidSpawner;
    private BoidSettings settings;
    private int id;

    private Material material;

    public Vector3 Position => transform.position;

    public Vector3 moveDirection { get; private set; }
    public Vector3 moveVelocity { get; private set; }
    private Vector3 moveAcceleration;

    private Vector3 separationSteerForce;
    private Vector3 alignmentSteerForce;
    private Vector3 cohesionSteerForce;

    private Vector3 obstacleAvoidanceDir;
    private Vector3 obstacleSteerForce;

    private Vector3 targetSteerForce;

    private int numNeighbourBoids;


    private delegate void OtherBoidInSightFunction(Boid other);


    private void OnDrawGizmos()
    {
        //DrawAvoidObstaclesDirection();
        return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + obstacleAvoidanceDir * settings.sightDistance);

        Gizmos.DrawSphere(transform.position + obstacleAvoidanceDir * settings.sightDistance, settings.spherecastRadius);
    }


    public void Init(BoidSpawner boidSpawner, BoidSettings settings, int id)
    {
        this.boidSpawner = boidSpawner;
        this.settings = settings;
        this.id = id;

        material = GetComponent<MeshRenderer>().material;

        moveDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        moveVelocity = moveDirection * settings.maxSpeed;
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

    public void ComputeState()
    {
        ClearState();
        LoopOtherBoidsInSight(SeparateAlignCohere);
    }

    public void UpdateState()
    {
        if (numNeighbourBoids > 0)
        {
            separationSteerForce /= numNeighbourBoids;
            separationSteerForce -= moveVelocity;

            alignmentSteerForce /= numNeighbourBoids;
            alignmentSteerForce -= moveVelocity;

            cohesionSteerForce /= numNeighbourBoids;
            cohesionSteerForce -= Position;

            //////
            
            moveAcceleration += separationSteerForce * settings.separationWeight;
            moveAcceleration += alignmentSteerForce * settings.alignmentWeight;
            moveAcceleration += cohesionSteerForce * settings.cohesionWeight;
        }

        // Obstacle avoidance
        obstacleAvoidanceDir = GetAvoidObstaclesDirection();
        obstacleSteerForce = obstacleAvoidanceDir * settings.maxSpeed - moveVelocity;

        moveAcceleration += obstacleSteerForce * settings.obstacleAvoidanceWeight;

        // Target
        if (settings.TargetExists)
        {
            targetSteerForce = (settings.TargetPosition - Position).normalized * settings.maxSpeed - moveVelocity;
            moveAcceleration += targetSteerForce * settings.targetWeight;
        }


        moveAcceleration = Vector3.ClampMagnitude(moveAcceleration * settings.maxForce, settings.maxAcceleration);

        moveVelocity += moveAcceleration * Time.deltaTime;
        moveVelocity = Vector3.ClampMagnitude(moveVelocity * settings.maxSpeed, settings.maxSpeed);

        UpdatePositionAndRotation();
    }


    private void ClearState()
    {
        moveDirection = moveVelocity.normalized;

        moveAcceleration = Vector3.zero;

        separationSteerForce = alignmentSteerForce = cohesionSteerForce = Vector3.zero;
        obstacleSteerForce = Vector3.zero;
        targetSteerForce = Vector3.zero;

        numNeighbourBoids = 0;
    }


    private void LoopOtherBoidsInSight(OtherBoidInSightFunction function)
    {
        for (int i = 0; i < id; ++i)
        {
            if (IsOtherBoidInSight(boidSpawner.boids[i]))
            {
                function(boidSpawner.boids[i]);
            }
        }

        for (int i = id + 1; i < boidSpawner.NumBoids; ++i)
        {
            if (IsOtherBoidInSight(boidSpawner.boids[i]))
            {
                function(boidSpawner.boids[i]);
            }
        }
    }

    private bool IsOtherBoidInSight(Boid other)
    {
        return Vector3.Distance(Position, other.Position) <= settings.sightDistance &&
               Vector3.Dot(moveDirection, (other.Position - Position).normalized) >= settings.sightThreshold;
    }

    private void UpdatePositionAndRotation()
    {
        transform.position = Position + moveVelocity * settings.maxSpeed * Time.deltaTime;
     
        //Quaternion look = Quaternion.LookRotation(Vector3.forward, moveDirection); //2D
        Quaternion look = Quaternion.LookRotation(moveDirection, transform.TransformDirection(Vector3.up));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, 400.0f * Time.deltaTime);
    }

    private void SeparateAlignCohere(Boid other)
    {
        ++numNeighbourBoids;

        // Separation
        Vector3 separation = (Position - other.Position);
        separationSteerForce += (separation.normalized * settings.sightDistance) - separation;

        // Alignment
        alignmentSteerForce += other.moveVelocity;

        // Cohesion
        cohesionSteerForce += other.Position;
    }

    private Vector3 GetAvoidObstaclesDirection()
    {
        Vector3 avoidDirection = moveDirection;
        float furthestObstacleDistance = 0.001f;

        RaycastHit hit;
        Vector3 hitDirection;

        for (int i = 0; i < BoidHelper.GetInstance().spherecastDirections.Count; ++i)
        {
            hitDirection = transform.TransformDirection(BoidHelper.GetInstance().spherecastDirections[i]);

            if (Physics.SphereCast(Position, settings.spherecastRadius, hitDirection, out hit, settings.sightDistance, 
                                   settings.obstacleProveMask, QueryTriggerInteraction.Ignore))
            {
                if (furthestObstacleDistance < hit.distance)
                {
                    furthestObstacleDistance = hit.distance;
                    avoidDirection = hitDirection;
                }
            }
            else
            {
                //if (id == 0) Debug.Log(i + " - " + hitDirection);
                return hitDirection;
            }
        }

        //if (id == 0) Debug.Log(avoidDirection);
        return avoidDirection;
    }


    private void DrawAvoidObstaclesDirection()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Position, Position + moveVelocity);

        Vector3 avoidDirection = moveDirection;
        float furthestObstacleDistance = 0.001f;

        RaycastHit hit;
        Vector3 hitDirection;

        for (int i = 0; i < BoidHelper.GetInstance().spherecastDirections.Count; ++i)
        {
            hitDirection = transform.TransformDirection(BoidHelper.GetInstance().spherecastDirections[i]);

            if (Physics.SphereCast(Position, settings.spherecastRadius, hitDirection, out hit, settings.sightDistance,
                                   settings.obstacleProveMask, QueryTriggerInteraction.Ignore))
            {
                if (furthestObstacleDistance < hit.distance)
                {
                    furthestObstacleDistance = hit.distance;
                    avoidDirection = hitDirection;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(Position, Position + avoidDirection * settings.sightDistance);
                }
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Position, Position + hitDirection * settings.sightDistance);
                return;
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Position, Position + avoidDirection * settings.sightDistance);
    }

}
