using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakPlatform : MonoBehaviour
{
    [SerializeField] private float timeToBreak = 2f;
    [SerializeField] private float timeToRespawn = 3f;

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private Sprite thisSprite;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisSprite = spriteRenderer.sprite;
    }
    private IEnumerator Breaking()
    {
        yield return new WaitForSeconds(timeToBreak);

        rb2D.simulated = false;
        spriteRenderer.sprite = null;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeToRespawn);

        rb2D.simulated = true;
        spriteRenderer.sprite = thisSprite;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Breaking());
        }
    }
}
