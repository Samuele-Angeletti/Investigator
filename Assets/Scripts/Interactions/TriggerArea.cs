using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] UnityEvent onTriggerEnter; 
    [SerializeField] UnityEvent onTriggerExit;

    public event Action OnTriggerEnterEvent;
    public event Action OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (interactableLayer.Contains(other.gameObject.layer))
        {
            onTriggerEnter?.Invoke();
            OnTriggerEnterEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactableLayer.Contains(other.gameObject.layer))
        {
            onTriggerExit?.Invoke();
            OnTriggerExitEvent?.Invoke();
        }
    }
}
