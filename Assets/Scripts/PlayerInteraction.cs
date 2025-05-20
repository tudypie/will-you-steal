using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private LayerMask interactionMask;
    [SerializeField] private float raycastSphereRadius = 2;

    private Item currentItem;

    private PlayerInventory inventory;
    private PlayerWrapping wrapping;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        wrapping = GetComponent<PlayerWrapping>();
    }

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
            currentItem = hits[0].GetComponent<Item>();

            if (currentItem.CanInteract)
            {
                currentItem.Focus();
            }
            else
            {
                currentItem.LoseFocus();
            }
        }
        else if (currentItem != null)
        {
            currentItem.LoseFocus();
            currentItem = null;
        }
    }

    private void HandleInput()
    {
        if (currentItem == null) { return; }

        if (!currentItem.CanInteract) { return; }

        if (!inventory.HasSpace(currentItem.Weight)) { return; }

        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        if (currentItem.TryGetComponent(out FragileItem fragileItem))
        {
            currentItem.CanInteract = false;
            wrapping.StartWrapping(fragileItem);
        }
        else
        {
            currentItem.Pickup();
            inventory.AddItem(currentItem);
            currentItem = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (raycastPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(raycastPoint.position, raycastSphereRadius);
    }
}
