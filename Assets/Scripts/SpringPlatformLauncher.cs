using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatformLauncher : MonoBehaviour
{
    [SerializeField] SpringJoint2D springJoint;

    float previousCompressedRatio;
    bool hasAlreadyStartedCompressing;
    bool hasAlreadyStartedDecompressing;


    private void Awake()
    {
        previousCompressedRatio = GetSpringJointCompressedRatio();
        hasAlreadyStartedCompressing = hasAlreadyStartedDecompressing = false;
    }


    bool StartsCompressingNow()
    {
        float currentCompressedRatio = GetSpringJointCompressedRatio();

        bool hasStartedCompressing = currentCompressedRatio > previousCompressedRatio && !hasAlreadyStartedCompressing;
        if (hasStartedCompressing)
        {
            hasAlreadyStartedCompressing = true;
            hasAlreadyStartedDecompressing = false;
        }

        previousCompressedRatio = currentCompressedRatio;

        return hasStartedCompressing;
    }

    bool StartsDecompressingNow()
    {
        float currentCompressedRatio = GetSpringJointCompressedRatio();

        bool hasStartedDecompressing = currentCompressedRatio < previousCompressedRatio && !hasAlreadyStartedCompressing;
        if (hasStartedDecompressing)
        {
            hasAlreadyStartedCompressing = false;
            hasAlreadyStartedDecompressing = true;
        }

        previousCompressedRatio = currentCompressedRatio;

        return hasStartedDecompressing;
    }




    // 0 = not compressed at all
    // 1 = fully compressed
    private float GetSpringJointCompressedRatio()
    {
        return 1.0f - (Vector2.Distance(transform.position, springJoint.connectedAnchor) / springJoint.distance);
    }

}
