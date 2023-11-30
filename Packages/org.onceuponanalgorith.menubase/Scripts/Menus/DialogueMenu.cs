using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DialogueDefinition
{
    public DialogueStep[] lines;
    public Texture2D other;
}

[Serializable]
public class DialogueStep
{
    public string text;
    public bool isMe;
}

public class DialogueMenu : Menu
{
    [SerializeField] private TextMeshProUGUI myWords;
    [SerializeField] private TextMeshProUGUI theirWords;
    [SerializeField] private RawImage theirImage;
    [SerializeField] private GameObject myPanel;
    [SerializeField] private GameObject theirPanel;
    private DialogueDefinition dialogueDef;
    private int step = 0;
    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
    }

    public void SetDialogue(DialogueDefinition def)
    {
        dialogueDef = def;
        step = 0;
        if (def.other == null)
        {
            theirImage.gameObject.SetActive(false);
        }
        else
        {
            theirImage.gameObject.SetActive(true);
            theirImage.texture = def.other;
        }
        Next();
    }

    public void Next()
    {
        if (step >= dialogueDef.lines.Length)
        {
            Close();
            return;
        }
        myPanel.SetActive(dialogueDef.lines[step].isMe);
        theirPanel.SetActive(!dialogueDef.lines[step].isMe);
        myWords.text = dialogueDef.lines[step].text;
        theirWords.text = dialogueDef.lines[step].text;
        step++;
    }

    public void Close()
    {
        manager.ShowMenu("Game");
        Time.timeScale = 1;
    }
}