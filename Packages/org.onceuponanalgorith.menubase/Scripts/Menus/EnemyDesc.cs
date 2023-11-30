using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class EnemyDesc : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public RawImage enemyImage;
    public TextMeshProUGUI descText;

    public void Activate(string name, Texture image, float attack, float range, float defense, float move, float maxHealth, float currentHealth) {
        nameText.text = "Name: " + name;
        enemyImage.texture = image;
        descText.text = "Attack: " + attack + "\nRange: " + range + "\nDefence: " + defense + "\nMove: " + move + "\nHealth: " + currentHealth + "/" + maxHealth;
    }
}