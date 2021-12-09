using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    private AuroraMovement auroraMovement;
    [SerializeField] private float bounceForce = 10f;

    private void Start()
    {
        auroraMovement = FindObjectOfType<AuroraMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //auroraMovement.AddForce(bounceForce);
        }
    }
}
