using UnityEngine;

public class VanInventory : MonoBehaviour
{
    [SerializeField] private float currentValue;

    public void DropItems(float totalValue)
    {
        Debug.Log($"{totalValue} money added to van");

        currentValue += totalValue;

        // play sound - drop items
    }
}
