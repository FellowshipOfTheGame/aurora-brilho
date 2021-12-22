using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [DraggablePoint] public Vector3[] spawnPositions;

    private AuroraMovement auroraMovement;

    private void Awake()
    {
        auroraMovement = FindObjectOfType<AuroraMovement>();
    }

    private void Start()
    {
        int spawnPosIndex = GameStateManager.instance.spawnIndex;
        Vector3 spawnPos;
        if (spawnPositions.Length <= spawnPosIndex)
        {
            Debug.Log("there isn't a spawn position with index " + spawnPosIndex + " in this scene");
            spawnPos = Vector2.zero;
        }
        else
        {
            spawnPos = spawnPositions[spawnPosIndex];
        }

        // spawn aurora in spawnPos
        auroraMovement.transform.position = spawnPos; // temporary solution
    }
}
