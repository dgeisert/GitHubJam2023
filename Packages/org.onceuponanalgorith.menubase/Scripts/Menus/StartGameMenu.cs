using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGameMenu : Menu
{
    [SerializeField] private CanvasGroup credits;
    [SerializeField] private CanvasGroup howToPlay;
    [SerializeField] private CanvasGroup settings;

    private Coroutine fadeMenu;
    private CanvasGroup currentCanvas;

    private void Start()
    {
        currentCanvas = howToPlay;
    }

    public void ShowCredits()
    {
        ShowText(credits);
    }
    public void ShowHowToPlay()
    {
        ShowText(howToPlay);
    }

    public void ShowText(CanvasGroup toGroup)
    {
        if (fadeMenu != null)
        {
            StopCoroutine(fadeMenu);
        }
        fadeMenu = StartCoroutine(DoShowText(currentCanvas, toGroup));
    }

    IEnumerator DoShowText(CanvasGroup fromGroup, CanvasGroup toGroup)
    {
        yield return null;
        float t = 0;
        while (t < 0.5f)
        {
            yield return null;
            t += Time.deltaTime;
            fromGroup.alpha = 1f - t * 2f;
            toGroup.alpha = t * 2f;
        }
        currentCanvas = toGroup;
    }

    public void StartGame()
    {
        manager.ShowMenu("Game");
        Time.timeScale = 1;
    }
    public void Settings()
    {
        manager.AddOverlay("Settings");
    }
}