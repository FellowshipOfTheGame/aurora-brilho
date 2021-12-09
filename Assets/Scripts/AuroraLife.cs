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
    private LifeItem[] lifesArray;

    void Start()
    {
        auroraRigidbody2D = GetComponent<Rigidbody2D>();
        lifeUI = FindObjectOfType<LifeUI>();
        changeLifeText = lifeUI.changeText;
        changeLifeText.Invoke(lives);

        lifesArray = FindObjectsOfType<LifeItem>();

        foreach (LifeItem lifeItem in lifesArray)
        {
            lifeItem.OnPickup += PickupLive;
        }
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

    private bool PickupLive()
    {
        if (lives < 3)
        {
            lives++;
            changeLifeText.Invoke(lives);
            return true;
        }

        return false;
    }

    public void Death()
    {
        // anima�ao de morte
        // nao destruir o objeto
        // mover ele pro checkpoint
        // respanwTime tem que ser maior que o tempo da anima�ao
        lives = 0;
        changeLifeText.Invoke(lives);
        StartCoroutine(Respawn(new Vector2(-14.8f, -12f)));
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
