using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float acceleration;
    [SerializeField] TriggerNotifyer groundTriggerNotifyer;

    private Vector2 moveDirection;

    public delegate void MovementAction();
    public static event MovementAction OnJump;



    private void Awake()
    {
    }


    void Start()
    {
        
    }


    void Update()
    {
        moveDirection.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && groundTriggerNotifyer.isInsideCollider)
        {
            Jump();
        }




    }


    private void FixedUpdate()
    {
        rb.AddForce(moveDirection * acceleration, ForceMode2D.Force);
        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed) 
        {
            Vector2 clampedRbVelocity = rb.velocity;
            clampedRbVelocity.x *= 0.9f;
            rb.velocity = clampedRbVelocity;
        }

    }


    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

}
