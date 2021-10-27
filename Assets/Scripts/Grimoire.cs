using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grimoire : MonoBehaviour
{
    public List<Collectable> collectables = new List<Collectable>();
    public List<string> loreStrings = new List<string> { "Teste", "Teste2" };

    [SerializeField] private Animator bookAnimation;
    private TMP_Text loreText;

    private void Awake()
    {
        loreText = GetComponentInChildren<TMP_Text>();
    }

    public void displayNewLore(int index)
    {
        loreText.text += "\n" + loreStrings[index];
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
}
