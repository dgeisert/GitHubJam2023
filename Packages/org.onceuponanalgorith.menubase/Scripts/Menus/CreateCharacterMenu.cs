using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateCharacterMenu : Menu
{
    [SerializeField] private TMP_InputField worldDescription;
    [SerializeField] private TMP_InputField name;
    [SerializeField] private TMP_InputField visualDescription;
    [SerializeField] private TextMeshProUGUI characterStory;
    [SerializeField] private RawImage fullImage;
    [SerializeField] private RawImage cutout;

    private string prefix = "A character in the center of the frame.  Full body render.  ";
    private string suffix = "(colorful), volumatic light.";

    public UnityEvent<string, string, string> setCharacterOnGenerate;
    public UnityEvent<string> setCharacterStory;
    public UnityEvent<Texture, Texture> setCharacterSprites;

    private void Start()
    {
        fullImage.texture = LoadImage("startChar_full");
        cutout.texture = LoadImage("startChar_cutout");
    }

    private Texture LoadImage(string filePath)
    {
        var tex = new Texture2D(512, 720);
        var bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/" + filePath + ".png");
        tex.LoadImage(bytes);
        return tex;
    }

    private void Update()
    {
        fullImage.color = new Color(1, 1, 1, Mathf.Sin(Time.time));
    }

    public void Generate()
    {
        canvasGroup.interactable = false;
        AIManager.Instance.GetImage(prefix + visualDescription.text, SetSprite, new AIPassthroughData());
        AIManager.Instance.GetText("Give a detailed and interesting backstory for the character named " +
            name.text +
            " with the description " +
            visualDescription.text +
            ", in the world of " + worldDescription.text + "." +
            " Be creative. But no more than 30 words.",
            SetStory,
            new AIPassthroughData());

        setCharacterOnGenerate.Invoke(worldDescription.text, name.text, visualDescription.text);
    }

    public void AcceptCharacter()
    {
        MenuManager.CharacterName = name.text;
        MenuManager.WorldDef = worldDescription.text;
        MenuManager.CharacterPhysicalDesc = visualDescription.text;
        manager.ShowMenu("Game");
        GameObject.FindObjectOfType<SelectionMenu>().SetCards();
    }

    private void SetImage(Texture cutoutSprite, AIPassthroughData passthroughData)
    {
        cutout.texture = cutoutSprite;
        canvasGroup.interactable = true;
    }
    private void SetSprite(Texture fullSprite, AIPassthroughData passthroughData)
    {
        fullImage.texture = fullSprite;
        canvasGroup.interactable = true;
        setCharacterSprites.Invoke(fullSprite, fullSprite);
    }
    private void SetStory(string story, AIPassthroughData data)
    {
        characterStory.text = story;
        setCharacterStory.Invoke(story);
    }

    public void RandomWorldDescription()
    {
        canvasGroup.interactable = false;
        string prompt = "Generate a world theme that fits in a battle game. Be creative. But no more than 15 words.";
        AIManager.Instance.GetText(
            prompt,
            SetWorldDescription,
            new AIPassthroughData());
    }
    public void RandomCharacterName()
    {
        canvasGroup.interactable = false;
        string prompt = "";
        if (worldDescription.text != "")
        {
            prompt += "Character's world description is " + worldDescription.text + ". ";
        }
        prompt += "Generate the character name that fits in a battle game. Be creative. But no more than 3 words.";
        AIManager.Instance.GetText(
            prompt,
            SetName,
            new AIPassthroughData());
    }
    public void RandomCharacterDescription()
    {
        canvasGroup.interactable = false;
        string prompt = "";
        if (name.text != "")
        {
            prompt += "Character name is " + name.text + ". ";
        }
        if (worldDescription.text != "")
        {
            prompt += "Character's world description is " + worldDescription.text + ". ";
        }
        prompt += "Generate the character appearance that fits in a battle game. Be creative. But no more than 15 words.";
        AIManager.Instance.GetText(
            prompt,
            SetAppearance,
            new AIPassthroughData());

    }
    private void SetWorldDescription(string worldDescription, AIPassthroughData data)
    {
        this.worldDescription.text = worldDescription.TrimStart();
        canvasGroup.interactable = true;
    }

    private void SetName(string name, AIPassthroughData data)
    {
        this.name.text = name;
        canvasGroup.interactable = true;
    }

    private void SetAppearance(string appearance, AIPassthroughData data)
    {
        this.visualDescription.text = appearance;
        canvasGroup.interactable = true;
    }
}