using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private Grimoire grimoire;

    private void Awake()
    {
        grimoire = FindObjectOfType<Grimoire>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            grimoire.collectablesCount++;

            uint quantity = grimoire.collectablesCount;
            grimoire.displayNewLore(quantity - 1, quantity);
            Destroy(gameObject);
        }
    }
}
