using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private AuroraLife auroraLife;
    private void Awake()
    {
        auroraLife = FindObjectOfType<AuroraLife>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            auroraLife.Death();
        }
    }

}
