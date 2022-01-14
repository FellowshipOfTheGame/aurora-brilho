using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public UnityAction<int> changeLivesUI;

    [SerializeField] private Sprite[] heartSprites;
    [SerializeField] private Image[] heartImages;

    // Start is called before the first frame update
    private void Awake()
    {
        changeLivesUI += ChangeLivesUI;
    }

    private void OnDestroy()
    {
        changeLivesUI -= ChangeLivesUI;
    }

    public void ChangeLivesUI(int lifeValue)
    {
        int maxLives = FindObjectOfType<AuroraLife>().MaxLives;

        for (int i = 0; i < maxLives; i++)
        {
            if (i < lifeValue)
            {
                heartImages[i].sprite = heartSprites[0];
            }
            else
            {
                heartImages[i].sprite = heartSprites[1];
            }
        }
    }
}
