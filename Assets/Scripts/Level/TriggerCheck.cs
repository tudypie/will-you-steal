using System;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayer;

    public event Action OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if ((triggerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            OnTrigger?.Invoke();
        }
    }
}
