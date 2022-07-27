using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidHelper
{
    static private BoidHelper instance;

    private int numSphereCastPoints = 100;
    private float turnFraction = 0.12f;

    private Vector3[] allSpherecastDirections;
    public List<Vector3> spherecastDirections { get; private set; }


    private BoidHelper()
    {
        ComputeSpherecastDirections();
    }

    static public BoidHelper GetInstance()
    {
        if (instance == null)
        {
            instance = new BoidHelper();
        }

        return instance;
    }


    public void ComputeSpherecastDirections()
    {
        allSpherecastDirections = new Vector3[numSphereCastPoints];

        for (int i = 0; i < numSphereCastPoints; ++i)
        {
            float t = i / (numSphereCastPoints - 1.0f);
            float inclination = Mathf.Acos(1.0f - 2.0f * t);
            float azimuth = 2.0f * Mathf.PI * turnFraction * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            allSpherecastDirections[i] = new Vector3(x, y, z);
        }
    }

    public void ValidateSpherecastDirections(float angleThreshold)
    {
        if (spherecastDirections == null) spherecastDirections = new List<Vector3>();
        else spherecastDirections.Clear();

        for (int i = 0; i < numSphereCastPoints; ++i)
        {
            if (Vector3.Dot(allSpherecastDirections[i], Vector3.forward) >= angleThreshold)
            {
                spherecastDirections.Add(allSpherecastDirections[i]);
            }
        }

    }



}
