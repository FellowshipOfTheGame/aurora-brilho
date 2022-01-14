using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [DraggablePoint] public Vector3 checkpointPosition;

    private AuroraMovement auroraMovement;

    private void Awake()
    {
        auroraMovement = FindObjectOfType<AuroraMovement>();
    }

    private void Start()
    {
        // spawn aurora in spawnPos
        if (GameStateManager.instance.spawnInCheckpoint)
        {
            auroraMovement.transform.position = checkpointPosition;
        }
    }

    public void SetCheckpoint()
    {
        GameStateManager.instance.checkpointSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
