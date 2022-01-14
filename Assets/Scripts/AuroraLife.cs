using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AuroraLife : MonoBehaviour
{
    private int maxLives = 3;
    public int currentLives;
    public bool canSufferDamage = true;
    private float invulnerabilityTime = 0f;
    private float time = 0;
    private Rigidbody2D auroraRigidbody2D;
    private AuroraMovement auroraMovement;
    private LifeUI lifeUI;
    private UnityAction<int> changeLifeText;
    private LifeItem[] lifesArray;
    public float knockbackForce;


    [Header("Cached components")]
    [SerializeField] Animator auroraAnimator;

    bool respawning = false;

    void Start()
    {
        if (GameStateManager.instance.spawnInCheckpoint)
        {
            currentLives = maxLives;
        }
        else
        {
            currentLives = GameStateManager.instance.auroraLives;
        }


        auroraRigidbody2D = GetComponent<Rigidbody2D>();
        auroraMovement = GetComponent<AuroraMovement>();
        lifeUI = FindObjectOfType<LifeUI>();
        if (lifeUI) changeLifeText = lifeUI.changeText;
        changeLifeText?.Invoke(currentLives);

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
        if (currentLives <= 0)
        {
            Death();
        }
    }

    private bool PickupLive()
    {
        if (currentLives < 3)
        {
            currentLives++;
            changeLifeText?.Invoke(currentLives);
            return true;
        }

        return false;
    }

    public void FullyHeal()
    {
        currentLives = 3;
        changeLifeText?.Invoke(currentLives);
    }

    public void Death()
    {
        if (!respawning)
        {
            currentLives = 0;
            changeLifeText?.Invoke(currentLives);
            auroraMovement.PauseMovement(true);
            canSufferDamage = false;
            auroraAnimator.SetTrigger("die");

            GameStateManager.instance.spawnInCheckpoint = true;
            SceneManagement.instance.LoadScene(GameStateManager.instance.checkpointSceneIndex);

            respawning = true;
        }
    }

    public void TakeDamage()
    {
        if (canSufferDamage)
        {
            currentLives--;
            canSufferDamage = false;
            auroraMovement.Knockback(knockbackForce);
            changeLifeText?.Invoke(currentLives);
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }
}
