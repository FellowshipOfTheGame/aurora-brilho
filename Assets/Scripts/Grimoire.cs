using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Grimoire : MonoBehaviour
{
    public List<Collectable> collectables = new List<Collectable>();
    //[SerializeField] private GameObject loreGameObject;
    //[SerializeField] private GameObject minimapGameObject;
    [SerializeField] private GameObject grimoireCanvas;
    [SerializeField] private Button menuButton;

    [SerializeField] private TMP_Text collectablesCountText;

    private Collectable[] collectablesArray;

    private void Awake()
    {
        menuButton.onClick.AddListener(delegate { FindObjectOfType<SceneManagement>().LoadMenu(); });
    }

    private void Start()
    {
        collectablesArray = FindObjectsOfType<Collectable>();

        foreach (Collectable collectable in collectablesArray)
        {
            collectable.OnPickup += HandlePickup;
        }

        UpdateCountText();
    }

    private void OnDestroy()
    {
        foreach (Collectable collectable in collectables)
        {
            collectable.OnPickup -= HandlePickup;
        }

        grimoireCanvas.GetComponentInChildren<Button>(true).onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            grimoireCanvas.SetActive(!grimoireCanvas.activeInHierarchy);
            //Time.timeScale = grimoireCanvas.activeInHierarchy ? 0f : 1f;
        }
    }

    public void BackButtonClick()
    {
        grimoireCanvas.SetActive(false);
    }

    private void HandlePickup(Collectable collectable)
    {
        collectables.Add(collectable);
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        collectablesCountText.text = collectables.Count.ToString() + "/" + collectablesArray.Length.ToString() + " Coletáveis coletados";
    }

    private void OnEnable()
    {
        //bookAnimation.SetBool("Open", true);
        //bookAnimation.SetBool("Close", false);
    }

    private void OnDisable()
    {
        //bookAnimation.SetBool("Open", false);
        //bookAnimation.SetBool("Close", true);
    }

    public void LoreClickButton()
    {
        // mudar a aparencia do botao
        //loreGameObject.SetActive(true);
        //minimapGameObject.SetActive(false);
    }

    public void MinimapClickButton()
    {
        // mudar a aparencia do botao
        //minimapGameObject.SetActive(true);
        //loreGameObject.SetActive(false);
    }
}
