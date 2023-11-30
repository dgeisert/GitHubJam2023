using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardLayout : MonoBehaviour
{
    [SerializeField] private Image BG;
    [SerializeField] private RawImage image;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI exp;
    public CardDefinition def;

    public void SetLayout(CardDefinition def)
    {
        this.def = def;
        title.text = def.cardName;
        description.text = def.description;
        image.texture = def.image;
        if(def.image != null)
        {
            image.color = Color.white;
        }
        BG.sprite = def.BG;
        BG.color = def.BGColor;
    }
}
