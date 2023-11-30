using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterNames
{
    Jorge,
    Hannah,
    Baha,
    Nathalie
}

public class PlayerCharacter : Character
{
    public CharacterNames cName;
    public float leftCooldown = 5f;
    private float lastLeft = -10;
    public float rightCooldown = 30f;
    private float lastRight = -10;
    private float lastDash = -10;
    public float dashCooldown = 10f;
    public float dashSpeed = 10f;
    public GameObject dashReady;
    public GameObject dashEffect;
    public Transform ring;
    private List<Enemy> enemyDamage;
    [SerializeField] private GameMenu gameMenu;
    [SerializeField] private DialogueMenu dialogueMenu;
    [SerializeField] private MenuManager menu;
    public float exp;
    private float expToLevel = 15;
    public int level = 1;
    private bool ready;
    void Start()
    {
        base.Start();
        enemyDamage = new List<Enemy>();
        SimpleCameraMovement.Instance.target = moveTarget;
        isEnemy = false;
        ready = current;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (!ready)
        {
            health = maxHealth;
            return;
        }
        SetUI();
        if (current)
        {
            ManualMove();
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame && DashCooldown() == 0)
        {
            StartCoroutine(Dash());
        }
        if (Mouse.current.leftButton.wasPressedThisFrame &&
            LeftCooldown() == 0 &&
            cName == CharacterNames.Hannah &&
            Time.timeScale > 0 &&
            gameMenu.cooldownL.gameObject.activeSelf)
        {
            LeftClick();
        }
        if (Mouse.current.rightButton.wasPressedThisFrame &&
            RightCooldown() == 0 &&
            gameMenu.cooldownR.gameObject.activeSelf)
        {
            RightClick();
        }
        if (exp >= expToLevel)
        {
            LevelUp();
        }
        if (!dashReady.activeSelf && DashCooldown() == 0)
        {
            dashReady.SetActive(true);
        }
        base.Update();
    }

    public void Ready()
    {
        ready = true;
        switch (cName)
        {
            case CharacterNames.Hannah:
                gameMenu.cooldownL.gameObject.SetActive(true);
                break;
            case CharacterNames.Nathalie:
                gameMenu.cooldownR.gameObject.SetActive(true);
                break;
        }
    }

    private void ManualMove()
    {
        ring.LookAt(CursorPlacement.Instance.transform, Vector3.up);
        mov = new Vector3(Keyboard.current.dKey.isPressed ? 1 : 0, 0, Keyboard.current.wKey.isPressed ? 1 : 0);
        mov -= new Vector3(Keyboard.current.aKey.isPressed ? 1 : 0, 0, Keyboard.current.sKey.isPressed ? 1 : 0);
        moveTarget.localPosition = mov;
    }

    IEnumerator Dash()
    {
        dashReady.SetActive(false);
        dashEffect.SetActive(false);
        dashEffect.SetActive(true);
        lastDash = Time.time;
        invincible = true;
        speed += dashSpeed;
        yield return new WaitForSeconds(0.2f);
        speed -= dashSpeed;
        invincible = false;
    }
    private void LeftClick()
    {
        lastLeft = Time.time;
        StartCoroutine(Flamethrower());
    }
    IEnumerator Flamethrower()
    {
        for (int i = 0; i < 60; i++)
        {
            yield return null;
            FireProjectile();
        }
    }
    private void RightClick()
    {
        lastRight = Time.time;
        UpdateHealth(maxHealth);
    }

    public float DashCooldown()
    {
        return 1 - Mathf.Clamp01((Time.time - lastDash) / dashCooldown);
    }
    public float LeftCooldown()
    {
        return 1 - Mathf.Clamp01((Time.time - lastLeft) / leftCooldown);
    }
    public float RightCooldown()
    {
        return 1 - Mathf.Clamp01((Time.time - lastRight) / rightCooldown);
    }
    private void SetUI()
    {
        switch (cName)
        {
            case CharacterNames.Jorge:
                gameMenu.cooldown1.SetCooldown(DashCooldown());
                gameMenu.playerInfo.SetEXP(exp / expToLevel);
                gameMenu.playerInfo.SetHealth(health / maxHealth);
                break;
            case CharacterNames.Hannah:
                gameMenu.cooldownL.SetCooldown(LeftCooldown());
                gameMenu.hannahInfo.SetEXP(exp / expToLevel);
                gameMenu.hannahInfo.SetHealth(health / maxHealth);
                break;
            case CharacterNames.Baha:
                gameMenu.bahaInfo.SetEXP(exp / expToLevel);
                gameMenu.bahaInfo.SetHealth(health / maxHealth);
                break;
            case CharacterNames.Nathalie:
                gameMenu.cooldownR.SetCooldown(RightCooldown());
                gameMenu.nathalieInfo.SetEXP(exp / expToLevel);
                gameMenu.nathalieInfo.SetHealth(health / maxHealth);
                break;
        }
    }
    private void LevelUp()
    {
        level++;
        exp -= expToLevel;
        expToLevel += level * 4;
        menu.ShowMenu("Selection");
    }
    private void OnCollisionStay(Collision other)
    {
        switch (other.collider.gameObject.name)
        {
            case "Enemy":
                Enemy e = other.collider.transform.parent.GetComponent<Enemy>();
                TakeDamage(e);
                break;
            default:
                Debug.Log(other.collider.gameObject.name);
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.collider.gameObject.name)
        {
            case "Enemy":
                Enemy e = other.collider.transform.parent.GetComponent<Enemy>();
                TakeDamage(e);
                break;
            default:
                Debug.Log(other.collider.gameObject.name);
                break;
        }
    }

    private void TakeDamage(Enemy e)
    {
        if (enemyDamage.Contains(e))
        {
            return;
        }
        else
        {
            if (UpdateHealth(-e.attack))
            {
                StartCoroutine(DamageCooldown(e));
            }
            FlashDamage();
        }
    }

    IEnumerator DamageCooldown(Enemy e)
    {
        enemyDamage.Add(e);
        yield return new WaitForSeconds(e.attackCooldown);
        enemyDamage.Remove(e);
    }

    protected override void Die()
    {
        base.Die();
        SetUI();
        if(current)
        {
            menu.ShowMenu("GameOver");
        }
    }

    public void ActivateCard(CardDefinition def)
    {
        switch (def.actionType)
        {
            case CardActionType.AttackVal:
                attack += def.val;
                break;
            case CardActionType.AttackSpeed:
                attackCooldown *= def.val;
                break;
            case CardActionType.Move:
                speed += def.val;
                break;
            case CardActionType.Health:
                maxHealth += def.val;
                UpdateHealth(def.val);
                break;
            case CardActionType.Regen:
                regen += def.val;
                break;
            case CardActionType.Heal:
                UpdateHealth(maxHealth);
                break;
            case CardActionType.Range:
                range += def.val;
                break;
            case CardActionType.DashCooldown:
                dashCooldown *= def.val;
                break;
            default:
                Debug.Log("Haven't set up : " + def.actionType.ToString());
                break;
        }
    }

    protected override void StartDialogue(DialogueDefinition def)
    {
        menu.ShowMenu("Dialogue");
        dialogueMenu.SetDialogue(def);
    }
}