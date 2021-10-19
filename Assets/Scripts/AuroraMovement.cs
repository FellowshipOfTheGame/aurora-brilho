using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuroraMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float friction;
    [SerializeField] float topSpeed;

    // Cached components
    Rigidbody2D rb;

    // Input variables
    int xInput;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xInput = GetHorizontalInput();
    }

    private void FixedUpdate()
    {
        if (xInput != 0)
        {
            bool changingDirections = xInput == 1 && rb.velocity.x < 0 || xInput == -1 && rb.velocity.x > 0;
            if (!changingDirections)
            {
                Run();
            }
            else
            {
                Reverse();
            }
        }
        else
        {
            ApplyFriction();
        }
    }

    private void Run()
    {
        float xVelocity = rb.velocity.x;

        // only move if not exceeded topSpeed
        if (Mathf.Abs(xVelocity) < topSpeed)
        {
            float velocityChange = acceleration * Time.deltaTime;
            xVelocity += Mathf.Sign(xVelocity) * velocityChange;
            xVelocity = Mathf.Clamp(xVelocity, -topSpeed, topSpeed);
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        }
    }

    private void Reverse()
    {
        float xVelocity = rb.velocity.x;
        float velocityChange = deceleration * Time.deltaTime;

        if (xVelocity > 0)
        {
            xVelocity -= velocityChange;
            if (xVelocity < 0) xVelocity = 0;
        }
        else
        {
            xVelocity += velocityChange;
            if (xVelocity > 0) xVelocity = 0;
        }

        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private void ApplyFriction()
    {
        float xVelocity = rb.velocity.x;
        float velocityChange = friction * Time.deltaTime;

        if (xVelocity > 0)
        {
            xVelocity -= velocityChange;
            if (xVelocity < 0) xVelocity = 0;
        }
        else
        {
            xVelocity += velocityChange;
            if (xVelocity > 0) xVelocity = 0;
        }

        rb.velocity = new Vector2(xVelocity, rb.velocity.y);
    }

    private int GetHorizontalInput()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");

        //Controller joystick snap
        xAxis = (xAxis > 0.1f) ? 1 : (xAxis < -0.1f) ? -1 : 0;

        return (int)xAxis;
    }
}
