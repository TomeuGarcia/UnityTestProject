using UnityEngine;

public class MovingSphere_Physics : MonoBehaviour
{
	[SerializeField] Transform playerInputSpace = default;

	[SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
	[SerializeField, Range(0f, 100f)] float maxAcceleration = 10f, maxAirAcceleration = 1f;
	[SerializeField, Range(0f, 10f)] float jumpHeight = 2f;
	[SerializeField, Range(0, 5)] int maxAirJumps = 0;
	[SerializeField, Range(0, 90)] float maxGroundAngle = 25f;
	[SerializeField, Range(0, 90)] float maxStairsAngle = 50f;
	[SerializeField, Range(0, 100)] float maxSnapSpeed = 100.0f;
	[SerializeField, Min(0.0f)] float probeDistance = 1.0f;
	[SerializeField] LayerMask probeMask = -1;
	[SerializeField] LayerMask stairsMask = -1;

	Rigidbody rb;
	Rigidbody connectedRb;
	Rigidbody previousConnectedRb;
	Renderer renderer;

	Vector3 velocity;
	Vector3 desiredVelocity;
	Vector3 connectionVelocity;

	Vector3 connectionWorldPosition;
	Vector3 connectionLocalPosition;

	Vector3 contactNormal;
	Vector3 steepNormal;

	Vector3 upAxis;
	Vector3 rightAxis;
	Vector3 forwardAxis;


	bool desiredJump = false;
	int groundContactCount = 0;
	int steepContactCount = 0;
	bool OnGround => groundContactCount > 0;
	bool OnSteep => steepContactCount > 0;
	int jumpPhase;
	int stepsSinceLastGrounded = 0;
	int stepsSinceLastJump = 0;

	float minGroundDotProduct;
	float minStairsDotProduct;


	void OnValidate()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
	}

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		renderer = GetComponent<Renderer>();
		OnValidate();
	}

	void Update()
	{
		Vector2 playerInput;
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);

		if (playerInputSpace)
        {
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        }
        else
        {
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}
		desiredVelocity = new Vector3(playerInput.x, 0.0f, playerInput.y) * maxSpeed;


		desiredJump |= Input.GetButtonDown("Jump");

		//renderer.material.SetColor("_BaseColor", OnGround ? Color.white : Color.black);
	}

	void FixedUpdate()
	{
		Vector3 gravity = CustomGravity.GetGravity(rb.position, out upAxis);

		UpdateState();
		AdjustVelocity();

		if (desiredJump)
		{
			desiredJump = false;
			Jump(gravity);
		}

		velocity += gravity * Time.deltaTime;

		rb.velocity = velocity;
		ClearState();
	}

	void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}


    void ClearState()
	{
		groundContactCount = 0;
		steepContactCount = 0;
		steepNormal = contactNormal = Vector3.zero;
		connectionVelocity = Vector3.zero;
		previousConnectedRb = connectedRb;
		connectedRb = null;
	}

	void UpdateState()
	{
		++stepsSinceLastGrounded;
		++stepsSinceLastJump;

		velocity = rb.velocity;
		if (OnGround || SnapToGround() || CheckSteepContacts())
		{
			stepsSinceLastGrounded = 0;

			jumpPhase = 0;
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = upAxis;
		}

		if (connectedRb)
        {
			if (connectedRb.isKinematic || connectedRb.mass >= rb.mass)
            {
				UpdateConnectionState();
			}
        }

	}

	void AdjustVelocity()
	{
		Vector3 xAxis = ProjectDirectionOnPlane(rightAxis, contactNormal);
		Vector3 zAxis = ProjectDirectionOnPlane(forwardAxis, contactNormal);

		Vector3 relativeVelocity = velocity - connectionVelocity;

		float currentX = Vector3.Dot(relativeVelocity, xAxis);
		float currentZ = Vector3.Dot(relativeVelocity, zAxis);

		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

		velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
	}

	void Jump(Vector3 gravity)
	{
		Vector3 jumpDirection;

		if (OnGround)
        {
			jumpDirection = contactNormal;
        }
		else if (OnSteep)
        {
			jumpDirection = steepNormal;
			jumpPhase = 0;
        }
		else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)
        {
			if (jumpPhase == 0)
            {
				jumpPhase = 1;
            }
			jumpDirection = contactNormal;
        }
        else
        {
			return;
        }


		stepsSinceLastJump = 0;
		if (stepsSinceLastJump > 1)
        {
			jumpPhase = 0;
        }

		jumpPhase += 1;
		float jumpSpeed = Mathf.Sqrt(2.0f * gravity.magnitude * jumpHeight);
		jumpDirection = (jumpDirection + upAxis).normalized;
		float alignedSpeed = Vector3.Dot(velocity, contactNormal);
		if (alignedSpeed > 0f)
		{
			jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
		}
		velocity += jumpDirection * jumpSpeed;
	}



	void EvaluateCollision(Collision collision)
	{
		float minDot = GetMinDot(collision.gameObject.layer);

		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			float upDot = Vector3.Dot(upAxis, normal);
			if (upDot >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
				connectedRb = collision.rigidbody;
			}
			else if (upDot > -0.01f)
            {
				++steepContactCount;
				steepNormal += normal;
				if (groundContactCount == 0)
				{
					connectedRb = collision.rigidbody;
				}
			}
		}
	}

	bool CheckSteepContacts()
    {
		if (steepContactCount > 1)
        {
			steepNormal.Normalize();
			float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot >= minGroundDotProduct)
            {
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
            }
        }
		return false;
    }


	Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}


	bool SnapToGround()
    {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) return false;

		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed) return false;

		if (!Physics.Raycast(rb.position, -upAxis, out RaycastHit hit, 
							 probeDistance, probeMask))
        {
			return false;
        }

		float upDot = Vector3.Dot(upAxis, hit.normal);
		if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
			return false;
        }

		groundContactCount = 1;
		contactNormal = hit.normal;

		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0.0f)
        {
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}

		connectedRb = hit.rigidbody;
		return true;
    }


	float GetMinDot(LayerMask layer)
    {
		return (stairsMask & (1 << layer)) == 0 ? minStairsDotProduct : minGroundDotProduct;
    }


	void UpdateConnectionState()
    {
		if (connectedRb == previousConnectedRb)
        {
			Vector3 connectionMovement = connectedRb.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
		}
		
		connectionWorldPosition = rb.position;
		connectionLocalPosition = connectedRb.transform.InverseTransformPoint(connectionWorldPosition);
	}

}