using UnityEngine;

public class Van : MonoBehaviour
{
    public bool PlayerIsNearVan { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsNearVan = true;
            other.GetComponent<PlayerInventory>().DropItemsToVan();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerIsNearVan = false;
        }
    }
}
