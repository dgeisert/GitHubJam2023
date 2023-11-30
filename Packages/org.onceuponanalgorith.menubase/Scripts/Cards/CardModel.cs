using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CardModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardDefinition def;
    public CardLayout layout;
    public CanvasGroup canvasGroup;

    public UnityEvent<CardDefinition, CardModel> OnSelected;
    private bool mouseOver;
    public bool isHand = true;
    private bool isSelected = false;
    private bool isDiscarded = false;
    private float hiddenCardY = -85f;
    private float discardedCardY = -200f;
    private float selectedCardY = 0;
    private float ShowCardY = 125f;

    private void Update()
    {
        if (isHand)
        {
            if (isDiscarded)
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, discardedCardY, 0.1f);
                transform.position = pos;
            }
            else if (mouseOver)
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, ShowCardY, 0.1f);
                transform.position = pos;
            }
            else if(isSelected)
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, selectedCardY, 0.1f);
                transform.position = pos;
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, hiddenCardY, 0.1f);
                transform.position = pos;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    private void Start()
    {
        if (def != null)
        {
            SetCard(def);
        }
    }

    public void Select()
    {
        OnSelected.Invoke(def, this);
        isSelected = true;
        isDiscarded = false;
        canvasGroup.interactable = true;
    }

    public void Deselect()
    {
        isSelected = false;
        isDiscarded = false;
        canvasGroup.interactable = true;
    }

    public void Discard()
    {
        isDiscarded = true;
        isSelected = false;
        canvasGroup.interactable = false;
    }

    public void SetCard(CardDefinition setDef)
    {
        def = setDef;
        layout.SetLayout(def);
    }
}