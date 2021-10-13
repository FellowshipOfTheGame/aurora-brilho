using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform follow;
    [SerializeField] private float delayVelocity = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, follow.position + offset, delayVelocity);
    }
}
