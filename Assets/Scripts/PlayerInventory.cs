using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private float carryLimit;

    [SerializeField, Space] private float currentCarryAmount;
    [SerializeField] private float currentValue;

    private void Start()
    {
        Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Van") && currentValue > 0)
        {
            VanInventory van = other.GetComponent<VanInventory>();
            van.DropItems(currentValue);
            Clear();
        }
    }

    private void Clear()
    {
        currentCarryAmount = 0;
        currentValue = 0;
    }

    public void AddItem(Item item)
    {
        currentCarryAmount += item.Weight;
        currentValue += item.Value;

        Debug.Log($"Item with weight = {item.Weight} and value = {item.Value} has been added to inventory");
    }

    public bool HasSpace(float weight)
    {
        if (currentCarryAmount + weight > carryLimit)
        {
            Debug.Log("Not enough space in inventory");
            // Display error text
            // play sound - error
            return false;
        }

        return true;
    }
}
