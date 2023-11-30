using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [HideInInspector]
    public MenuManager manager;
    private Animator animator;
    protected CanvasGroup canvasGroup;
    public bool isEnabled = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void HideImmediate()
    {
        canvasGroup.interactable = false;
        animator.SetTrigger("HideImmediate");
    }
    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        animator.SetTrigger("HideMenu");
    }
    public virtual void Show()
    {
        canvasGroup.interactable = true;
        isEnabled = true;
        animator.SetTrigger("ShowMenu");
    }
    public virtual void Disable()
    {
        if(!isEnabled)
        {
            return;
        }
        canvasGroup.interactable = false;
        isEnabled = false;
        animator.SetTrigger("Disable");
    }
    public virtual void Enable()
    {
        canvasGroup.interactable = true;
        isEnabled = true;
        animator.SetTrigger("Enable");
    }
}
