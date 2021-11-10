using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuroraLife : MonoBehaviour
{
    public uint lives = 3;
    public bool canSufferDamage = true;
    private float invulnerabilityTime = 3f;
    private float time = 0;
    private Rigidbody2D auroraRigidbody2D;
    private LifeUI lifeUI;

    void Start()
    {
        auroraRigidbody2D = GetComponent<Rigidbody2D>();
        lifeUI = FindObjectOfType<LifeUI>();
        lifeUI.ChangeText(lives);
    }

    private void Update()
    {
        if (!canSufferDamage)
        {
            time += Time.deltaTime;
        }
        if (time >= invulnerabilityTime)
        {
            time = 0f;
            canSufferDamage = true;
        }
    }

    void FixedUpdate()
    {
        if (lives <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        // animaçao de morte
        // nao destruir o objeto
        // mover ele pro checkpoint
        gameObject.SetActive(false);
    }

    public void TakeDamage()
    {
        if (canSufferDamage)
        {
            lives--;
            canSufferDamage = false;
            auroraRigidbody2D.AddForce(new Vector2(30f, 30f), ForceMode2D.Impulse);
            lifeUI.ChangeText(lives);
        }
    }
}
