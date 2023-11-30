using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileAim
{
    Mouse,
    Player,
    Movement,

}

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float damage;
    public bool isEnemy;
    [SerializeField] private Texture2D tex;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider col;
    [SerializeField] private ProjectileAim aim;
    [SerializeField] private float random;

    // Start is called before the first frame update
    public void Init(Character ch)
    {
        damage = ch.attack;
        switch (aim)
        {
            case ProjectileAim.Mouse:
                transform.LookAt(CursorPlacement.Instance.transform.position, Vector3.up);
                break;
            case ProjectileAim.Player:
                transform.LookAt(ch.moveTarget, Vector3.up);
                break;
            case ProjectileAim.Movement:
                transform.LookAt(ch.moveTarget, Vector3.up);
                break;
        }
        if (random > 0)
        {
            transform.forward += random * new Vector3((Random.value - 0.5f), 0, (Random.value - 0.5f));
        }
        transform.position += transform.forward * 0.5f;
        isEnemy = ch.isEnemy;
        if (!isEnemy)
        {
            col.transform.localScale *= 2f;
        }
        meshRenderer.material.SetTexture("_Sprite", tex);
        meshRenderer.material.SetFloat("_sway", 0);
        Destroy(gameObject, ch.range / speed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public void Hit()
    {
        Destroy(gameObject);
    }
}