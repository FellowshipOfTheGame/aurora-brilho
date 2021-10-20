using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuroraMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float topSpeed;
    [SerializeField] [Range(0, 1)] float airControl;

    [Header("Jump variables")]
    [SerializeField] float jumpHeight;
    [SerializeField] float timeToApex;
    const int jumpsQuantity = 2;
    float gravity;

    [Header("Ground check")]
    [SerializeField] LayerMask groundMask;
    const float groundCheckWidth = 0.015f; // tested value

    // Current state related variables
    bool grounded;
    int jumpsAvailable;

    // Cached components
    Rigidbody2D rb;
    BoxCollider2D boxCollider;

    // Input variables
    int xAxisInput;
    bool jumpKey;

    [Header("Debug")]
    [SerializeField] bool recalculateVariables;

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Initialize jump related variables
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        rb.gravityScale = gravity / Physics2D.gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        GetHorizontalInput();
        GetJumpInput();
    }

    private void FixedUpdate()
    {
        CheckGrounded(); // updates grounded variable

        if (grounded)
            jumpsAvailable = jumpsQuantity; // reset jumps

        if (jumpKey)
        {
            jumpKey = false; // resets key even if not able to jump
            if (jumpsAvailable > 0)
            {
                Jump();
                jumpsAvailable--;
            }
        }

        HorizontalMovement();
    }

    private void Jump()
    {
        #region Debug
        if (recalculateVariables)
        {
            gravity = -2 * jumpHeight / (timeToApex * timeToApex);
            rb.gravityScale = gravity / Physics2D.gravity.y;
        }
        #endregion

        // y velocity is set to jump force, current velocity is overwritten
        rb.velocity = new Vector2(rb.velocity.x, -gravity * timeToApex);
    }

    private void HorizontalMovement()
    {
        bool inputInDirectionOfMovement = (xAxisInput == 1 && rb.velocity.x >= 0) || (xAxisInput == -1 && rb.velocity.x <= 0);

        if (inputInDirectionOfMovement && Mathf.Abs(rb.velocity.x) <= topSpeed)
        {
            Accelerate();
        }
        else
        {
            Decelerate();
        }
    }

    private void Accelerate()
    {
        float xVelocity = rb.velocity.x;

        float velocityChange = acceleration * Time.deltaTime;
        if (!grounded) velocityChange *= airControl;

        if (xAxisInput == 1)
            xVelocity += velocityChange;
        else
            xVelocity -= velocityChange;

        xVelocity = Mathf.Clamp(xVelocity, -topSpeed, topSpeed);
        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void Decelerate()
    {
        float xVelocity = rb.velocity.x;

        float velocityChange = deceleration * Time.deltaTime;
        if (!grounded) velocityChange *= airControl;

        if (xVelocity > 0)
        {
            xVelocity -= velocityChange;
            if (xVelocity < 0) xVelocity = 0;
        }
        else if (xVelocity < 0)
        {
            xVelocity += velocityChange;
            if (xVelocity > 0) xVelocity = 0;
        }

        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void CheckGrounded()
    {
        Vector2 pointA, pointB;

        pointA = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x + groundCheckWidth,
            boxCollider.bounds.center.y - boxCollider.bounds.extents.y + groundCheckWidth);
        pointB = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x - groundCheckWidth,
            boxCollider.bounds.center.y - boxCollider.bounds.extents.y - groundCheckWidth);

        if (Physics2D.OverlapArea(pointA, pointB, groundMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    #region Input Functions

    private void GetHorizontalInput()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");

        // Controller joystick snap
        xAxis = (xAxis > 0.1f) ? 1 : (xAxis < -0.1f) ? -1 : 0;

        xAxisInput = (int)xAxis;
    }

    private void GetJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpKey = true;
        }
    }

    #endregion

}
