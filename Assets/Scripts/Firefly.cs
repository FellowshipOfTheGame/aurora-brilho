using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform follow;
    [SerializeField] private float smoothTime = 0.3f;

    //[SerializeField] private float delayVelocity = 0.07f;

    private Vector3 currentOffset;
    private Vector3 yVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        currentOffset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow.position.x > transform.position.x)
        {
            currentOffset = new Vector2(offset.x, offset.y);
        }
        else
        {
            currentOffset = new Vector2(-offset.x, offset.y);
        }

        Movement();
    }

    private void Movement()
    {

        // Flip
        if (follow.position.x >= transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, -180, 0);

        // Move
        Vector3 targetposition = follow.position + currentOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref yVelocity, smoothTime);
    }
}
