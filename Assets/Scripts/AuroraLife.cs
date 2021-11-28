using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AuroraLife : MonoBehaviour
{
    public uint lives = 3;
    public bool canSufferDamage = true;
    private float invulnerabilityTime = 3f;
    private float time = 0;
    private Rigidbody2D auroraRigidbody2D;
    private LifeUI lifeUI;
    private float respawnTime = 2f;
    private UnityAction<uint> changeLifeText;

    void Start()
    {
        auroraRigidbody2D = GetComponent<Rigidbody2D>();
        lifeUI = FindObjectOfType<LifeUI>();
        changeLifeText = lifeUI.changeText;
        changeLifeText.Invoke(lives);
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

    public void Death()
    {
        // animaçao de morte
        // nao destruir o objeto
        // mover ele pro checkpoint
        // respanwTime tem que ser maior que o tempo da animaçao
        lives = 0;
        changeLifeText.Invoke(lives);
        StartCoroutine(Respawn(new Vector2(-14.8f, -13f)));
    }

    public IEnumerator Respawn(Vector2 respawnPosition)
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = respawnPosition;
        lives = 3;
        changeLifeText.Invoke(lives);
    }

    public void TakeDamage(Vector2 directionForce)
    {
        if (canSufferDamage)
        {
            lives--;
            canSufferDamage = false;
            auroraRigidbody2D.AddForce(directionForce, ForceMode2D.Impulse);
            changeLifeText.Invoke(lives);
        }
    }
}
