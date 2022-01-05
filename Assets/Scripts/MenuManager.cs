using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] private GameObject creditsGameObject;

    public void OpenCredits()
    {
        menuGameObject.SetActive(false);
        creditsGameObject.SetActive(true);
    }
    public void CloseCredits()
    {
        creditsGameObject.SetActive(false);
        menuGameObject.SetActive(true);
    }
}
