using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class LifeUI : MonoBehaviour
{
    private TMP_Text lifeText;
    private AuroraLife auroraLife;
    public UnityAction<int> changeText;

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

    public void ChangeText(int lifeValue)
    {
        lifeText.text = lifeValue.ToString();
    }
}
