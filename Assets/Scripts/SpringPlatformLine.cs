using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatformLine : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] SpringJoint2D springJoint;


    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, springJoint.connectedAnchor);
    }

}
