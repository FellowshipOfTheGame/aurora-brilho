using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    private void Awake()
    {
        #region Singleton Pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        #endregion

        checkpointSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public bool spawnInCheckpoint = true;
    public int spawnIndex = 0;
    public int checkpointSceneIndex;

    public int auroraLives;
}
