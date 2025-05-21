using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerMovement Movement;

    public static PlayerInteraction Interaction;

    public static PlayerInventory Inventory;

    public static PlayerWrapping Wrapping;

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
        Interaction = GetComponent<PlayerInteraction>();
        Inventory = GetComponent<PlayerInventory>();
        Wrapping = GetComponent<PlayerWrapping>();
    }
}
