using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CardAction
{
    public CardActionType type;
    public float range = 1;
    public float AOE = 0;
    public float value = 1;
}
public enum CardActionType
{
    None,
    AttackVal,
    AttackSpeed,
    Health,
    Heal,
    Range,
    Regen,
    Move,
    AOE,
    DashCooldown,
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/CardDefinition", order = 1)]
public class CardDefinition : ScriptableObject
{
    public string cardName;
    public string description;
    public CardActionType actionType;
    public float val;
    public Texture image;
    public Sprite BG;
    public Color BGColor = Color.white;
    public List<CardDefinition> unlockCards;
    public string audio = "";

    public void Generate()
    {
        //no generating right now
    }

    public void SetSprite(Texture fullSprite, AIPassthroughData passthroughData)
    {
        SaveImage("Card_" + name, (Texture2D) fullSprite);
        image = fullSprite;
        foreach (CardLayout cl in GameObject.FindObjectsOfType<CardLayout>())
        {
            if (cl.def == this)
            {
                cl.SetLayout(cl.def);
            }
        }
    }

    public void LoadSprites()
    {
        if (File.Exists(Application.streamingAssetsPath + "/Card_" + name + ".png"))
        {
            image = LoadImage("Card_" + name);
        }
        else
        {
            //GenerateCard();
        }
    }

    private Texture LoadImage(string filePath)
    {
        var tex = new Texture2D(198, 120);
        var bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/" + filePath + ".png");
        tex.LoadImage(bytes);
        return tex;
    }

    private void SaveImage(string filePath, Texture2D tex)
    {
        File.WriteAllBytes(Application.streamingAssetsPath + "/" + filePath + ".png", tex.EncodeToPNG());
    }

    public void SetCardName(string name, AIPassthroughData passthroughData)
    {
        cardName = name;
        AIManager.Instance.GetImage(cardName + ". " + MenuManager.CharacterPhysicalDesc, SetSprite, new AIPassthroughData());
    }
}