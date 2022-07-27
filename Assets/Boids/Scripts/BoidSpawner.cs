using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private Boid boidPrefab;
    [SerializeField] private int numSpawns = 10;
    [SerializeField] bool setFirstBoidAsTarget = false;

    [SerializeField] Color aColor = Color.cyan;
    [SerializeField] Color bColor = Color.red;

    public Boid[] boids { get; private set; }
    public int NumBoids => boids.Length;

    [SerializeField] BoidSettings settings;

    const float maxX = 10.0f;
    const float maxY = 15.0f;
    const float maxZ = 10.0f;


    private void OnValidate()
    {
        settings.Update();
        BoidHelper.GetInstance().ValidateSpherecastDirections(settings.sightThreshold);
    }


    private void Awake()
    {
        const float maxSpawnCoord = 2.0f;
        Vector3 spawnPosition;


        boids = new Boid[numSpawns];

        for (int i = 0; i < numSpawns; ++i)
        {
            spawnPosition = new Vector3(Random.Range(-maxSpawnCoord + spawnTransform.position.x, maxSpawnCoord + spawnTransform.position.x), 
                                        Random.Range(-maxSpawnCoord + spawnTransform.position.y, maxSpawnCoord + spawnTransform.position.y),
                                        Random.Range(-maxSpawnCoord + spawnTransform.position.z, maxSpawnCoord + spawnTransform.position.z));

            boids[i] = Instantiate(boidPrefab, spawnPosition, Quaternion.identity, spawnTransform);
            boids[i].Init(this, settings, i);
            boids[i].SetColor(new Color(Random.Range(aColor.r, bColor.r),
                                        Random.Range(aColor.g, bColor.g),
                                        Random.Range(aColor.b, bColor.b),
                                        1.0f));
        }

        if (setFirstBoidAsTarget && numSpawns > 0)
        {
            settings.target = boids[0].transform;
            boids[0].SetColor(Color.green);
        }
    }


    private void Update()
    {
        for (int i = 0; i < boids.Length; ++i)
        {
            //KeepInsideBox(boids[i].transform);
            boids[i].ComputeState();
        }

        for (int i = 0; i < boids.Length; ++i)
        {
            boids[i].UpdateState();
        }
    }


    private void KeepInsideBox(Transform boidTransform)
    {
        if (boidTransform.position.x > maxX)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.x = -maxX;
            boidTransform.position = newPosition;
        }
        else if (boidTransform.position.x < -maxX)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.x = maxX;
            boidTransform.position = newPosition;
        }

        if (boidTransform.position.y > maxY)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.y = 0.0f;
            boidTransform.position = newPosition;
        }
        else if (boidTransform.position.y < 0.0f)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.y = maxY;
            boidTransform.position = newPosition;
        }

        if (boidTransform.position.z > maxZ)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.z = -maxZ;
            boidTransform.position = newPosition;
        }
        else if (boidTransform.position.z < -maxZ)
        {
            Vector3 newPosition = boidTransform.position;
            newPosition.z = maxZ;
            boidTransform.position = newPosition;
        }

    }

}
