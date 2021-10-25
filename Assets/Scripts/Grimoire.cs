using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Grimoire : MonoBehaviour
{
    static public List<Collectable> collectables;
    static public List<string> loreStrings = new List<string> { "Teste", "Teste2" };

    [SerializeField] private Animator bookAnimation;
    static private TMP_Text loreText;

    static public void displayNewLore(int index)
    {
        loreText.text += "\n" + loreStrings[index];
    }

    private void OnEnable()
    {
        bookAnimation.SetBool("Open", true);
        bookAnimation.SetBool("Close", false);
    }

    private void OnDisable()
    {
        bookAnimation.SetBool("Open", false);
        bookAnimation.SetBool("Close", true);
    }
}
