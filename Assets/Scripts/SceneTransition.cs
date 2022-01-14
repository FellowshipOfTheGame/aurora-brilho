using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private int destinationSpawnIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameStateManager.instance.auroraLives = FindObjectOfType<AuroraLife>().GetCurrentLives();
            GameStateManager.instance.spawnInCheckpoint = false;
            GameStateManager.instance.spawnIndex = destinationSpawnIndex;
            FindObjectOfType<SceneManagement>().LoadScene(sceneName);
        }
    }
}
