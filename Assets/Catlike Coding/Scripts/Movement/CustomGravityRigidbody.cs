
using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class CustomGravityRigidbody : MonoBehaviour
{
    [SerializeField] bool floatToSleep = false;

    Rigidbody rb;
    float floatDelay = 0.0f;

    Material material;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        material = GetComponent<MeshRenderer>().material;
    }


    private void FixedUpdate()
    {
        if (floatToSleep)
        {
            if (rb.IsSleeping())
            {
                floatDelay = 0.0f;
                material.SetColor("_BaseColor", Color.gray);
                return;
            }

            if (rb.velocity.sqrMagnitude < 0.0001f)
            {
                floatDelay += Time.deltaTime;
                material.SetColor("_BaseColor", Color.yellow);
                if (floatDelay >= 1.0f) return;
            }
            else
            {
                floatDelay = 0.0f;
            }
        }
        

        material.SetColor("_BaseColor", Color.red);
        rb.AddForce(CustomGravity.GetGravity(rb.position), ForceMode.Acceleration);
    }

}
