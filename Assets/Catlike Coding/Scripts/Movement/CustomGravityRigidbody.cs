
using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class CustomGravityRigidbody : MonoBehaviour
{
    [SerializeField] bool floatToSleep = false;

    [SerializeField] float submergenceOffset = 0.5f;
    [SerializeField, Min(0.1f)] float submergenceRange = 1f;
    [SerializeField, Min(0f)] float buoyancy = 1f;
    [SerializeField, Range(0f, 10f)] float waterDrag = 1f;
    [SerializeField] LayerMask waterMask = 0;
    [SerializeField] Vector3 buoyancyOffset = new Vector3(0.0f, 0.01f, 0.0f);


    Rigidbody rb;
    float floatDelay = 0.0f;

    Material material;

    float submergence;
    Vector3 gravity;


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


        gravity = CustomGravity.GetGravity(rb.position);
        if (submergence > 0.0f)
        {
            float drag = Mathf.Max(0.0f, 1.0f - waterDrag * submergence * Time.deltaTime);
            rb.velocity *= drag;
            rb.angularVelocity *= drag;
            rb.AddForceAtPosition(gravity * -(buoyancy * submergence), transform.TransformPoint(buoyancyOffset), ForceMode.Acceleration);

            submergence = 0.0f;
        }
        rb.AddForce(gravity, ForceMode.Acceleration);
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!rb.IsSleeping() && (waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence();
        }
    }

    void EvaluateSubmergence()
    {
        Vector3 upAxis = -gravity.normalized;


        if (Physics.Raycast(rb.position + upAxis * submergenceOffset, -upAxis, out RaycastHit hit, submergenceRange + 1.0f,
            waterMask, QueryTriggerInteraction.Collide))
        {
            submergence = 1.0f - hit.distance / submergenceRange;
        }
        else
        {
            submergence = 1.0f;
        }
    }
}
