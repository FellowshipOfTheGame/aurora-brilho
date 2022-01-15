using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] float transitionTime = 1f;

    IEnumerator LoadLevel(int index)
    {
        Animator transition = GameObject.Find("/Level Loader/Crossfade")?.GetComponent<Animator>();
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }

    IEnumerator LoadLevel(string index)
    {
        Animator transition = GameObject.Find("/Level Loader/Crossfade")?.GetComponent<Animator>();
        transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }

    public void LoadMenu()
    {
        Destroy(GameStateManager.instance.gameObject);

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
