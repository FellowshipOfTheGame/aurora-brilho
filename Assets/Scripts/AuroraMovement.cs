using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuroraMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    float acceleration;
    float deceleration;
    [SerializeField] float accelerationTime;
    [SerializeField] float decelerationTime;
    [SerializeField] float topSpeed;
    [SerializeField] [Range(0, 1)] float airControl;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight;
    [SerializeField] float timeToApex;
    const int jumpsQuantity = 2;
    float gravity;

    [Header("Wall Jump")]
    [SerializeField] float wallSilidingSpeed;
    [SerializeField] float wallJumpAngle;
    bool walledLeft, walledRight;
    bool wallSliding;

    [Header("Ground Check")]
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

        // Initialize horizontal movement variables
        acceleration = topSpeed / accelerationTime;
        deceleration = topSpeed / decelerationTime;
    }

    // Update is called once per frame
    void Update()
    {
        GetHorizontalInput();
        GetJumpInput();

        if (recalculateVariables)
        {
            RecalculateVariables();
        }
    }

    private void FixedUpdate()
    {
        CheckGrounded(); // updates grounded variable
        CheckWalled(); // updates walled variables

        if (grounded || walledRight || walledLeft)
            jumpsAvailable = jumpsQuantity; // reset jumps
        else if (jumpsAvailable == jumpsQuantity)
            jumpsAvailable--;

        if ((walledRight || walledLeft) && !grounded)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

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
        float jumpVelocity = -gravity * timeToApex;

        if (!wallSliding)
        {
            // y velocity is set to jump force, current velocity is overwritten
            rb.velocity = new Vector2(rb.velocity.x, -gravity * timeToApex);
        }
        else
        {
            if (walledLeft)
                rb.velocity = new Vector2(jumpVelocity * Mathf.Cos(wallJumpAngle * Mathf.Deg2Rad), jumpVelocity * Mathf.Sin(wallJumpAngle * Mathf.Deg2Rad));
            else
                rb.velocity = new Vector2(-jumpVelocity * Mathf.Cos(wallJumpAngle * Mathf.Deg2Rad), jumpVelocity * Mathf.Sin(wallJumpAngle * Mathf.Deg2Rad));
        }
        
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
            if (xVelocity < 0 && xAxisInput == 0) xVelocity = 0;
        }
        else if (xVelocity < 0)
        {
            xVelocity += velocityChange;
            if (xVelocity > 0 && xAxisInput == 0) xVelocity = 0;
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

    private void CheckWalled()
    {
        Vector2 pointA, pointB;

        // walled left
        pointA = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x - groundCheckWidth,
            boxCollider.bounds.center.y + boxCollider.bounds.extents.y - groundCheckWidth);
        pointB = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x + groundCheckWidth,
            boxCollider.bounds.center.y - boxCollider.bounds.extents.y + groundCheckWidth);

        if (Physics2D.OverlapArea(pointA, pointB, groundMask))
        {
            walledLeft = true;
        }
        else
        {
            walledLeft = false;
        }

        // walled right
        pointA = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x - groundCheckWidth,
            boxCollider.bounds.center.y + boxCollider.bounds.extents.y - groundCheckWidth);
        pointB = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x + groundCheckWidth,
            boxCollider.bounds.center.y - boxCollider.bounds.extents.y + groundCheckWidth);

        if (Physics2D.OverlapArea(pointA, pointB, groundMask))
        {
            walledRight = true;
        }
        else
        {
            walledRight = false;
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

    void RecalculateVariables()
    {
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        rb.gravityScale = gravity / Physics2D.gravity.y;

        acceleration = topSpeed / accelerationTime;
        deceleration = topSpeed / decelerationTime;
    }
}
