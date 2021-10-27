using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform follow;
    [SerializeField] private float delayVelocity = 0.07f;

    private Vector3 currentOffset;

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
            currentOffset.x = offset.x;
        }
        else
        {
            currentOffset.x = -offset.x;
        }

        Movement();
    }

    private void Movement()
    {
        transform.position = new Vector2(Mathf.SmoothStep(transform.position.x, follow.position.x + currentOffset.x, delayVelocity),
                    Mathf.SmoothStep(transform.position.y, follow.position.y + currentOffset.y, delayVelocity));
    }
}
