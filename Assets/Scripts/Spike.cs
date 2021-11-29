using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private AuroraLife auroraLife;

    private void Start()
    {
        auroraLife = FindObjectOfType<AuroraLife>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //auroraLife.TakeDamage(collision.relativeVelocity);
            auroraLife.TakeDamage(collision.rigidbody.velocity);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            auroraLife.TakeDamage(collision.rigidbody.velocity);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            auroraLife.TakeDamage(collision.rigidbody.velocity);
        }
    }
}
