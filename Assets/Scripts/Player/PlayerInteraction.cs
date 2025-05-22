using System.Linq;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private LayerMask interactionMask;
    [SerializeField] private float raycastSphereRadius = 2;

    private Item currentItem;

    private void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        Collider[] hits = Physics.OverlapSphere(raycastPoint.position, raycastSphereRadius, interactionMask);

        if (hits.Length > 0)
        {
            float closestDistance = float.MaxValue;
            Item closestItem = null;

            foreach (Collider hit in hits)
            {
                Item item = hit.GetComponentInParent<Item>();

                float distance = Vector3.Distance(raycastPoint.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }

            if (currentItem != closestItem)
            {
                if (currentItem != null)
                {
                    currentItem.LoseFocus();
                }

                currentItem = closestItem;
                currentItem.Focus();
            }
        }
        else if (hits.Length == 0 && currentItem != null)
        {
            currentItem.LoseFocus();
            currentItem = null;
        }
    }


    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentItem != null)
        {
            currentItem.Interact();
        }
    }

    private void OnDrawGizmos()
    {
        if (raycastPoint == null) { return; }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(raycastPoint.position, raycastSphereRadius);
    }
}
