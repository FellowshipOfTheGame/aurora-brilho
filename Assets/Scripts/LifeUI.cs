using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeUI : MonoBehaviour
{
    private TMP_Text lifeText;
    private AuroraLife auroraLife;

    // Start is called before the first frame update
    void Start()
    {
        lifeText = GetComponent<TMP_Text>();
        auroraLife = FindObjectOfType<AuroraLife>();
    }

    public void ChangeText(uint lifeValue)
    {
        lifeText.text = lifeValue.ToString();
    }
}
