using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LifeItem : MonoBehaviour
{
    public event Func<bool> OnPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (OnPickup())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
