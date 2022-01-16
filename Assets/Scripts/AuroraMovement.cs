using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuroraMovement : MonoBehaviour
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
    [SerializeField] float topFallingSpeed;
    float gravityScale;
    float gravity;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight;
    [SerializeField] float timeToApex;
    const float coyoteTime = 0.1f;
    const float jumpToleranceInput = 0.08f;
    const int jumpsQuantity = 2;

    [Header("Wall Movement")]
    [SerializeField] float wallSilidingTopSpeed;
    [SerializeField] float wallJumpAngle;
    [SerializeField] float leaveWallToleranceTime;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    const float groundCheckWidth = 0.015f; // tested value -> 0.015f

    [Header("Knockback")]
    [SerializeField] float knockbackAngle;
    [SerializeField] float knockbackDuration;


    // Current state of the game
    bool grounded;
    float coyoteTimePassed;
    int jumpsAvailable;
    bool walledLeft, walledRight;
    bool wallSliding;
    float wallInputTiming;
    bool jumping, jumpCutEarly;
    bool facingRight = true;
    bool canMove = true;
    bool knockedBack = false;
    float knockbackTimer = 0f;
    bool stopInput = false;

    [Header("Cached components")]
    [SerializeField] Animator animator;
    [SerializeField] Transform modelTransform; 
    Rigidbody2D rb;
    BoxCollider2D boxCollider;

    // Input variables
    int xAxisInput;
    public int XAxisInput { set => xAxisInput = value; }
    float jumpKeyPressedInTime = -1f;
    bool jumpKeyReleased;

    [Header("Debug")]
    [SerializeField] bool recalculateVariables;

    private void Awake()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        InitializeVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopInput)
        {
            GetHorizontalInput();
            GetJumpInput();
        }

        if (recalculateVariables)
        {
            InitializeVariables();
        }

    }

    private void FixedUpdate()
    {
        if (knockedBack)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackDuration)
            {
                knockedBack = false;
            }
        }
        else if (canMove)
        {
            CheckGrounded(); // updates grounded variable
            CheckWalled(); // updates walled variables

            // Wall slide
            if ((walledRight || walledLeft) && !grounded)
            {
                wallSliding = true;
                WallSlide();
            }
            else
            {
                wallSliding = false;
                wallInputTiming = 0f;
            }

            // Coyote time
            if (grounded)
            {
                coyoteTimePassed = coyoteTime;
            }
            else
            {
                coyoteTimePassed -= Time.deltaTime;
            }

            if (grounded || wallSliding)
            {
                jumpsAvailable = jumpsQuantity; // reset jumps
                jumping = false;
                jumpCutEarly = false;
            }
            else if (jumpsAvailable == jumpsQuantity && coyoteTimePassed < 0)
                jumpsAvailable--; // lose one jump if not walled and if not grounded after coyoteTime has passed

            // It takes care of horizontal movement on ground and on air
            if (!wallSliding)
                HorizontalMovement();

            if (jumpKeyPressedInTime >= Time.time && jumpsAvailable > 0)
            {
                Jump();
                jumpsAvailable--;

                if (!grounded && !wallSliding) // for animation purpose only
                    animator.SetTrigger("doubleJump");

                jumping = true;

                // reset variables
                jumpKeyPressedInTime = -1f;
                jumpCutEarly = false;
            }

            if (jumping && jumpKeyReleased)
            {
                jumpCutEarly = true;
            }
            jumpKeyReleased = false;


            // Handles vertical velocit and acceleration
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
                if (rb.velocity.y < -topFallingSpeed) // limit falling speed
                    rb.velocity = new Vector2(rb.velocity.x, -topFallingSpeed);
            }

            SetAnimationParameters();
            FlipSprite();
        }
    }

    public void Knockback(float force)
    {
        float direction = facingRight ? -1 : 1;
        rb.velocity = new Vector2(direction * force * Mathf.Cos(knockbackAngle * Mathf.Deg2Rad), force * Mathf.Sin(knockbackAngle * Mathf.Deg2Rad));

        animator.SetFloat("m_knockback", 1 / knockbackDuration);
        animator.SetTrigger("knockback");

        knockedBack = true;
        knockbackTimer = 0f;
    }

    public void Bounce(Vector2 direction, float force)
    {
        SoundManager.instance.PlaySound("Bounce");
        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        jumpsAvailable = jumpsQuantity - 1;

        if (!grounded && !wallSliding) // for animation purpose only
            animator.SetTrigger("doubleJump");

        jumping = false;

        // reset variables
        jumpKeyPressedInTime = -1f;
        jumpCutEarly = false;
    }

    public void StopInput(bool stopIt)
    {
        if (stopIt)
        {
            xAxisInput = 0;
            jumpKeyPressedInTime = -1f;
            jumpKeyReleased = false;

            stopInput = true;
        }
        else
        {
            stopInput = false;
        }
    }

    public void PauseMovement(bool pauseIt)
    {
        if (pauseIt)
        {
            rb.simulated = false;
            wallSliding = false;
            wallInputTiming = 0f;

            jumping = false;
            jumpsAvailable = 0;
            jumpCutEarly = false;
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.simulated = true;
        }

        canMove = !pauseIt;
    }

    private void FlipSprite()
    {
        if (wallSliding)
        {
            facingRight = walledRight ? true : false;
        }
        else if (rb.velocity.x > 0)
        {
            facingRight = true;
        }
        else if (rb.velocity.x < 0)
        {
            facingRight = false;
        }

        if (facingRight)
            modelTransform.rotation = Quaternion.Euler(0, 0, 0);
        else
            modelTransform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void SetAnimationParameters()
    {
        animator.SetBool("grounded", grounded);
        animator.SetBool("walled", wallSliding);
        animator.SetFloat("xVelocity", rb.velocity.x);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void WallSlide()
    {
        // Limit vertical velocity while wall sliding
        float ySpeed = (rb.velocity.y < -wallSilidingTopSpeed) ? -wallSilidingTopSpeed : rb.velocity.y;
        rb.velocity = new Vector2(rb.velocity.x, ySpeed);

        // To leave wall you have to hold input button for a certain time
        if (xAxisInput == 1 && walledLeft || xAxisInput == -1 && walledRight)
        {
            wallInputTiming += Time.deltaTime;
        }
        else
        {
            wallInputTiming = 0f;
        }

        if (wallInputTiming >= leaveWallToleranceTime)
        {
            wallSliding = false; // it means you can move horizontally now and are not wall sliding anymore
        }
    }

    private void Jump()
    {
        //SoundManager.instance.PlaySound("Jump");

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
            jumpKeyPressedInTime = Time.time + jumpToleranceInput;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpKeyReleased = true;
        }
    }

    #endregion

    void InitializeVariables()
    {
        // Initialize jump related variables
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        gravityScale = gravity / Physics2D.gravity.y;

        // Initialize horizontal movement variables
        acceleration = topSpeed / accelerationTime;
        deceleration = topSpeed / decelerationTime;
    }
}
