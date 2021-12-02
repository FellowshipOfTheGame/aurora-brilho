using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class LifeUI : MonoBehaviour
{
    private TMP_Text lifeText;
    private AuroraLife auroraLife;
    public UnityAction<uint> changeText;

    // Start is called before the first frame update
    private void Awake()
    {
        lifeText = GetComponent<TMP_Text>();
        auroraLife = FindObjectOfType<AuroraLife>();
        changeText += ChangeText;
    }

    private void OnDestroy()
    {
        changeText -= ChangeText;
    }

    public void ChangeText(uint lifeValue)
    {
        lifeText.text = lifeValue.ToString();
    }
}
