using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownArea : MonoBehaviour
{
    [SerializeField] private RawImage BG;
    [SerializeField] private Image ring;
    public void SetCooldown(float cooldown)
    {
        if (cooldown == 0)
        {
            BG.color = Color.white;
        }
        else
        {
            BG.color = Color.grey;
        }
        ring.fillAmount = cooldown;
    }
}