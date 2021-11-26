using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    private SceneManagement sceneManagement;

    private void Awake()
    {
        sceneManagement = SceneManagement.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            sceneManagement.LoadScene(sceneIndex);
        }
    }
}
