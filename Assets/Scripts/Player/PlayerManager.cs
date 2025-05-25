using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerMovement Movement;

    public static PlayerInteraction Interaction;

    public static PlayerInventory Inventory;

    public static PlayerWrapping Wrapping;

    public static PlayerManager Instance;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Movement = GetComponent<PlayerMovement>();
        Interaction = GetComponent<PlayerInteraction>();
        Inventory = GetComponent<PlayerInventory>();
        Wrapping = GetComponent<PlayerWrapping>();
    }

    public void DisablePlayer()
    {
        Movement.enabled = false;
        Interaction.enabled = false;
        Wrapping.StopWrapping();
    }
}
