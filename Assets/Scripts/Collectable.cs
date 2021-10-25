using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ref List<Collectable> collectablesList = ref Grimoire.collectables;
            collectablesList.Add(this);
            Grimoire.displayNewLore(collectablesList.Count - 1);
            Destroy(gameObject);
        }
    }
}
