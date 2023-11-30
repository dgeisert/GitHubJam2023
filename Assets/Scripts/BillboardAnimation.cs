using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardAnimation : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private Texture2D setTex;
    private Coroutine flashAnimCorout;
    private Transform cam;
    private bool isDead = false;
    public bool isStatic = false;
    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    private void Start()
    {
        cam = Camera.main.transform;
        if (setTex != null)
        {
            SetTexture(setTex);
        }
    }
    void Update()
    {
        if (isDead || isStatic)
        {
            return;
        }
        transform.LookAt(new Vector3(cam.position.x, (transform.position.y + cam.position.y) / 2, cam.position.z - 0.5f));
    }

    public void SetTexture(Texture2D tex)
    {
        mat.SetTexture("_Sprite", tex);
    }

    public void AnimateFlash()
    {
        if (flashAnimCorout != null)
        {
            StopCoroutine(flashAnimCorout);
        }
        flashAnimCorout = StartCoroutine(FlashAnimation());
    }
    IEnumerator FlashAnimation()
    {
        float t = 0;
        while (t < 0.25f)
        {
            mat.SetFloat("_Damage", 0.5f - t * 2f);
            t += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_Damage", 0);
    }

    public void Die()
    {
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        isDead = true;
        float t = 0;
        yield return null;
        if (flashAnimCorout != null)
        {
            StopCoroutine(flashAnimCorout);
        }
        while (t < 1)
        {
            mat.SetFloat("_Damage", 0.75f - t / 2f);
            transform.parent.eulerAngles = new Vector3(t * 89, 0, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }
}