using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private Grimoire grimoire;

    private void Awake()
    {
        grimoire = FindObjectOfType<Grimoire>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            grimoire.collectables.Add(this);
            grimoire.displayNewLore(grimoire.collectables.Count - 1);
            Destroy(gameObject);
        }
    }
}
