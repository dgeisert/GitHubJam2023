using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private RawImage portrait;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform expBar;

    public void SetEXP(float exp)
    {
        expBar.sizeDelta = new Vector2(100 * exp, 10);
    }
    public void SetHealth(float health)
    {
        healthBar.sizeDelta = new Vector2(100 * health, 10);
    }
}