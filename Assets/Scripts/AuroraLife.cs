using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AuroraLife : MonoBehaviour
{
    public uint lives = 3;
    public bool canSufferDamage = true;
    private float invulnerabilityTime = 0f;
    private float time = 0;
    private Rigidbody2D auroraRigidbody2D;
    private AuroraMovement auroraMovement;
    private LifeUI lifeUI;
    private float respawnTime = 2f;
    private UnityAction<uint> changeLifeText;
    private LifeItem[] lifesArray;
    public float knockbackForce;

    private Animator crossfade;

    [Header("Cached components")]
    [SerializeField] Animator auroraAnimator;

    bool respawning = false;
    float respawnTimer;

    void Start()
    {
        crossfade = GameObject.Find("/Level Loader/Crossfade")?.GetComponent<Animator>();

        auroraRigidbody2D = GetComponent<Rigidbody2D>();
        auroraMovement = GetComponent<AuroraMovement>();
        lifeUI = FindObjectOfType<LifeUI>();
        if (lifeUI) changeLifeText = lifeUI.changeText;
        changeLifeText?.Invoke(lives);

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

        if (respawning)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > respawnTime)
            {
                respawning = false;
                canSufferDamage = true;
                lives = 3;
                changeLifeText?.Invoke(lives);
                
                auroraMovement.PauseMovement(false);

                auroraRigidbody2D.position = FindObjectOfType<SpawnManager>().GetSpawnPosition();
                auroraRigidbody2D.velocity = Vector2.zero;

                crossfade?.SetTrigger("end");

                auroraAnimator.SetTrigger("revive");
            }
        }
    }

    private bool PickupLive()
    {
        if (lives < 3)
        {
            lives++;
            changeLifeText?.Invoke(lives);
            return true;
        }

        return false;
    }

    public void Death()
    {
        if (!respawning)
        {
            // animaçao de morte
            // nao destruir o objeto
            // mover ele pro checkpoint
            // respanwTime tem que ser maior que o tempo da animaçao
            lives = 0;
            changeLifeText?.Invoke(lives);
            auroraMovement.PauseMovement(true);
            canSufferDamage = false;
            respawning = true;
            respawnTimer = 0f;

            crossfade?.SetTrigger("start");

            auroraAnimator.SetTrigger("die");
        }
    }

    public void TakeDamage()
    {
        if (canSufferDamage)
        {
            lives--;
            canSufferDamage = false;
            auroraMovement.Knockback(knockbackForce);
            changeLifeText?.Invoke(lives);
        }
    }
}
