using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapWings : MonoBehaviour
{
    public Transform wing1, wing2;
    private Vector3 centered1, centered2;
    public MeshRenderer mr;
    void Start()
    {
        wing1.GetComponent<MeshRenderer>().material = mr.material;
        wing2.GetComponent<MeshRenderer>().material = mr.material;
        centered1 = wing1.localEulerAngles;
        centered2 = wing2.localEulerAngles;
        StartCoroutine(Flap());
    }

    IEnumerator Flap()
    {
        float random = Random.value * 4 + 1;
        for (int i = 0; i < random; i++)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                wing1.localEulerAngles = centered1 - new Vector3(Mathf.Sin(t * 6) * 50f, 0, 0);
                wing2.localEulerAngles = centered2 - new Vector3(Mathf.Sin(t * 6) * 50f, 0, 0);
                yield return null;
            }
        }
        wing1.localEulerAngles = centered1;
        wing2.localEulerAngles = centered2;
        yield return new WaitForSeconds(Random.value * 3f);
        StartCoroutine(Flap());
    }
}