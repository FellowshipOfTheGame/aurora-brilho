using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuGameObject;
    [SerializeField] private GameObject creditsGameObject;

    private AuroraMovement auroraMovement;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Awake()
    {
        auroraMovement = FindObjectOfType<AuroraMovement>();
        auroraMovement.StopInput(true);
        soundManager = SoundManager.instance;
    }

    public void PlaySound(string soundName)
    {
        soundManager.PlaySound(soundName);
    }

    public void HideMenuCanvas()
    {
        menuGameObject.SetActive(false);
    }

    public void MoveAurora(int input)
    {
        auroraMovement.XAxisInput = input;
    }

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
