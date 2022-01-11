using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static int index = 0;

    [SerializeField] float transitionTime = 1f;

    public static SceneManagement instance; // singleton

    private void Awake()
    {
        #region Singleton
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
    }

    IEnumerator LoadLevel(int index)
    {
        Animator transition = GameObject.Find("/Level Loader/Crossfade").GetComponent<Animator>();
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }

    IEnumerator LoadLevel(string index)
    {
        Animator transition = GameObject.Find("/Level Loader/Crossfade").GetComponent<Animator>();
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    public void LoadScene(string name)
    {
        StartCoroutine(LoadLevel(name));
    }

    public void LoadTutorial()
    {
        StartCoroutine(LoadLevel("0-1"));
    }

    public void Exit()
    {
        Application.Quit();
    }
}
