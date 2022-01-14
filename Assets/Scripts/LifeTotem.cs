using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTotem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // cura Aurora
            FindObjectOfType<AuroraLife>().FullyHeal();

            // Seta checkpoint atual a este
            GetComponentInChildren<CheckpointManager>().SetCheckpoint();
        }
    }
}
