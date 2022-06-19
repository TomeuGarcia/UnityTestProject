using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float jumpSpeed;
    private Vector2 moveDirection;

    [SerializeField] float acceleration;

    [SerializeField] TriggerNotifyer groundTriggerNotifyer;


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
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }

    }


    private void FixedUpdate()
    {
        //if (pressedJump)
        //{
        //    Debug.Log("Jump");
        //    rb.AddForce(Vector2.up * 3000.0f, ForceMode2D.Impulse);
        //    pressedJump = false;
        //}

        //Vector2 movement = moveDirection * moveSpeed;// * Time.fixedDeltaTime;
        //rb.MovePosition(rb.position + movement);
        //rb.velocity = movement;


        rb.AddForce(moveDirection * acceleration, ForceMode2D.Force);
        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed) 
        {
            Vector2 clampedRbVelocity = rb.velocity;
            clampedRbVelocity.x *= 0.9f;
            rb.velocity = clampedRbVelocity;
        }

    }


}
