using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorujaMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float accelerationTime;
    [SerializeField] float decelerationTime;
    [SerializeField] float topSpeed;
    [SerializeField] [Range(0, 1)] float airControl;
    float acceleration;
    float deceleration;

    [Header("Vertical Movement")]
    [SerializeField] float fallMultiplier;
    [SerializeField] float lowJumpMultiplier;
    float gravityScale;
    float gravity;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight;
    [SerializeField] float timeToApex;
    const float coyoteTime = 0.1f;
    const float jumpToleranceInput = 0.08f;
    const int jumpsQuantity = 3;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    const float groundCheckWidth = 0.015f; // tested value -> 0.015f

    // Current state of the game
    bool grounded;
    float coyoteTimePassed;
    int jumpsAvailable;
    float jumpInputTiming;
    bool jumping, jumpCutEarly;
    bool facingRight;

    // Cached components
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    Animator animator;
    SpriteRenderer spriteRenderer;

    // Input variables
    int xAxisInput;
    bool jumpKeyPressed;
    bool jumpKeyReleased;

    [Header("Debug")]
    [SerializeField] bool recalculateVariables;

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize jump related variables
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        gravityScale = gravity / Physics2D.gravity.y;
        rb.gravityScale = gravityScale;

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

        if (grounded)
        {
            coyoteTimePassed = coyoteTime;
        }
        else
        {
            coyoteTimePassed -= Time.deltaTime;
        }

        if (grounded)
        {
            jumpsAvailable = jumpsQuantity; // reset jumps
            jumping = false;
            jumpCutEarly = false;
        }
        else if (jumpsAvailable == jumpsQuantity && coyoteTimePassed < 0)
            jumpsAvailable--; // lose one jump if not walled and if not grounded after coyoteTime has passed

        // It takes care of horizontal movement on ground and on air
        HorizontalMovement();

        if (jumpKeyPressed)
        {
            if (jumpsAvailable > 0)
            {
                Jump();
                jumpsAvailable--;
                jumping = true;

                // reset variables
                jumpKeyPressed = false;
                jumpInputTiming = jumpToleranceInput;
                jumpCutEarly = false;
            }
            else
            {
                jumpInputTiming -= Time.deltaTime;
                if (jumpInputTiming < 0)
                {
                    jumpKeyPressed = false;
                    jumpInputTiming = jumpToleranceInput;
                }
            }
        }

        if (jumping && jumpKeyReleased)
        {
            jumpCutEarly = true;
        }
        jumpKeyReleased = false;

        if (rb.velocity.y >= 0)
        {
            if (jumpCutEarly)
                rb.gravityScale = gravityScale * lowJumpMultiplier;
            else
                rb.gravityScale = gravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale * fallMultiplier;
        }

        SetAnimationParameters();
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (rb.velocity.x > 0)
        {
            facingRight = true;
        }
        else if (rb.velocity.x < 0)
        {
            facingRight = false;
        }

        spriteRenderer.flipX = !facingRight;
    }

    private void SetAnimationParameters()
    {
        animator.SetBool("grounded", grounded);
        animator.SetFloat("xVelocity", rb.velocity.x);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void Jump()
    {
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
            jumpKeyPressed = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpKeyReleased = true;
        }
    }

    #endregion

    void RecalculateVariables()
    {
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        gravityScale = gravity / Physics2D.gravity.y;

        acceleration = topSpeed / accelerationTime;
        deceleration = topSpeed / decelerationTime;
    }
}
