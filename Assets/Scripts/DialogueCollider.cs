using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueCollider : MonoBehaviour
{
    public DialogueDefinition def;
    [SerializeField] private Collider col;
    public UnityEvent onTrigger;

    public void Trigger()
    {
        col.enabled = false;
        onTrigger.Invoke();
    }
}
