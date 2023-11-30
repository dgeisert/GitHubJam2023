using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class GameMenu : Menu
{
    public CooldownArea cooldown1;
    public CooldownArea cooldownR;
    public CooldownArea cooldownL;
    public PlayerInfo playerInfo;
    public PlayerInfo hannahInfo;
    public PlayerInfo bahaInfo;
    public PlayerInfo nathalieInfo;
    private int round;
    public CanvasGroup roundAnimGroup;
    public UnityEvent OnNextRound;

    public void Reset()
    { }

    public void Pause()
    {
        Disable();
        manager.AddOverlay("Pause");
    }
    public void GameOver()
    {
        manager.ShowMenu("GameOver");
    }

    public void NextRound()
    {
        //trigger the next round
        StartCoroutine(NextRoundAnim());
        OnNextRound.Invoke();
    }

    IEnumerator NextRoundAnim()
    {
        //next round animation
        canvasGroup.interactable = false;
        round++;
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            roundAnimGroup.alpha += t / 0.3f;
            yield return null;
        }
        roundAnimGroup.alpha = 1;
        yield return new WaitForSeconds(2);
        t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            yield return null;
            roundAnimGroup.alpha -= t / 0.3f;
        }
        roundAnimGroup.alpha = 0;
        canvasGroup.interactable = true;
    }
}