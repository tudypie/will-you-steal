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
        // Create a sphere in front of player
        Collider[] hits = Physics.OverlapSphere(raycastPoint.position, raycastSphereRadius, interactionMask);

        // If there are any items in the sphere
        if (hits.Length > 0)
        {
            // Calculate distance for each item and find minimum

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

            // If closest item is a new one
            if (currentItem != closestItem)
            {
                // Stop focusing on previous item
                if (currentItem != null)
                {
                    currentItem.LoseFocus();
                }

                // Set new current item and focus
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
        // Draw a debug gizmos to view the sphere in editor

        if (raycastPoint == null) { return; }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(raycastPoint.position, raycastSphereRadius);
    }
}
