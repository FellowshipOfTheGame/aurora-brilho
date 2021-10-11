using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private Grimoire grimoire;

    private void Awake()
    {
        grimoire = Grimoire.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            grimoire.collectables.Add(this);
            Destroy(gameObject);
        }
    }
}
