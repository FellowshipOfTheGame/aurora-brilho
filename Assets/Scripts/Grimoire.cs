using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Grimoire : MonoBehaviour
{
    public List<Collectable> collectables = new List<Collectable>();
    //public string[] loreStrings = new string[] { "Teste", "Teste2", "Teste3", "Teste4", "Teste5", "Teste6", "Teste7", "Teste8", "Teste9", "Teste10", "Teste11" };

    // [SerializeField] private Animator bookAnimator = null;
    [SerializeField] private GameObject loreGameObject;
    [SerializeField] private GameObject minimapGameObject;
    [SerializeField] private GameObject grimoireCanvas;
    //[SerializeField] private Button pageLeftButton;
    //[SerializeField] private Button pageRightButton;

    private TMP_Text collectablesCountText;

    //private uint currentPage = 1;
    //private const int textPerPage = 5;
    private Collectable[] collectablesArray;

    private void Awake()
    {
        collectablesCountText = loreGameObject.GetComponentInChildren<TMP_Text>(true);
    }

    private void Start()
    {
        collectablesArray = FindObjectsOfType<Collectable>();

        foreach (Collectable collectable in collectablesArray)
        {
            collectable.OnPickup += HandlePickup;
        }
    }

    private void OnDestroy()
    {
        foreach (Collectable collectable in collectables)
        {
            collectable.OnPickup -= HandlePickup;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            grimoireCanvas.SetActive(!grimoireCanvas.activeInHierarchy);
            Time.timeScale = grimoireCanvas.activeInHierarchy ? 0f : 1f;
        }
    }

    private void HandlePickup(Collectable collectable)
    {
        collectables.Add(collectable);
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        collectablesCountText.text = collectables.Count.ToString() + "/" + collectablesArray.Length.ToString();
    }
    /*
    public void displayNewLore(uint inicio, uint fim)
    {
        for (uint i = inicio; i < fim; i++)
        {
            collectablesCountText.text += loreStrings[i] + "\n";
        }
    }*/

    /*
    private void ChangePage(uint pageNumber, uint lorePerPage)
    {
        collectablesCountText.text = "";
        displayNewLore(lorePerPage * (pageNumber - 1), lorePerPage * pageNumber - 1);
    }*/
    /*
    public void PageRight()
    {
        if (collectablesCount > currentPage * textPerPage)
        {
            currentPage++;
            ChangePage(currentPage, textPerPage);

            if (collectablesCount > currentPage * textPerPage)
            {
                pageRightButton.interactable = false;
            }
            else
            {
                pageRightButton.interactable = true;
            }
        }
    }*/
    /*
    public void PageLeft()
    {
        if (currentPage > 1)
        {
            currentPage--;
            ChangePage(currentPage, textPerPage);

            if (currentPage > 1)
            {
                pageLeftButton.interactable = false;
            }
            else
            {
                pageLeftButton.interactable = true;
            }
        }
    }*/

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
        loreGameObject.SetActive(true);
        minimapGameObject.SetActive(false);
    }

    public void MinimapClickButton()
    {
        // mudar a aparencia do botao
        minimapGameObject.SetActive(true);
        loreGameObject.SetActive(false);
    }
}
