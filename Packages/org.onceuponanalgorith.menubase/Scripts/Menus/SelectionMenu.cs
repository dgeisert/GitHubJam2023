using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;
using System;

public class SelectionMenu : Menu
{
    public CardModel card0;
    public CardModel card1;
    public CardModel card2;

    public List<CardDefinition> startCardPool;
    public List<CardDefinition> cardPool;
    public List<CardDefinition> claimedCards;
    public List<CardDefinition> generatedCards;
    
    public UnityEvent<CardDefinition> ActivateCard;

    public void Reset()
    {
        cardPool = new List<CardDefinition>();
        claimedCards = new List<CardDefinition>();
        generatedCards = new List<CardDefinition>();
        AddToPool(startCardPool);
    }

    public void SelectCard(int i)
    {
        manager.ShowMenu("Game");
        switch (i)
        {
            case 0:
                claimedCards.Add(card0.def);
                AddToPool(card0.def.unlockCards);
                ActivateCard.Invoke(card0.def);
                break;
            case 1:
                claimedCards.Add(card1.def);
                AddToPool(card1.def.unlockCards);
                ActivateCard.Invoke(card1.def);
                break;
            case 2:
                claimedCards.Add(card2.def);
                AddToPool(card2.def.unlockCards);
                ActivateCard.Invoke(card2.def);
                break;
        }
        Time.timeScale = 1;
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0;
        SetCards();
    }

    public void AddToPool(List<CardDefinition> newCards)
    {
        foreach (CardDefinition cd in newCards)
        {
            cardPool.Add(cd);
        }
    }

    public void SetCards()
    {
        if (cardPool.Count < 3)
        {
            Debug.LogError("Out of Cards for Card Pool!!!");
        }
        Random random = new Random();
        for (int i = cardPool.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            CardDefinition temp = cardPool[i];
            cardPool[i] = cardPool[j];
            cardPool[j] = temp;
        }
        CheckGenerateCard(cardPool[0]);
        CheckGenerateCard(cardPool[1]);
        CheckGenerateCard(cardPool[2]);
        card0.SetCard(cardPool[0]);
        card1.SetCard(cardPool[1]);
        card2.SetCard(cardPool[2]);
    }

    public void CheckGenerateCard(CardDefinition cd)
    {
        if (!generatedCards.Contains(cd))
        {
            generatedCards.Add(cd);
            cd.Generate();
        }
    }
}