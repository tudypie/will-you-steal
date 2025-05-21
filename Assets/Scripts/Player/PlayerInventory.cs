using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int carryLimit;

    [SerializeField, Space] private int currentCarryAmount;
    [SerializeField] private int currentValue;

    private bool firstItemWasAdded = false;

    private void Start()
    {
        Clear();
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

        if (!firstItemWasAdded)
        {
            LevelManager.Instance.StartCountdown();
            firstItemWasAdded = true;
        }
    }

    public void DropItemsToVan()
    {
        if (currentValue <= 0) { return; }

        LevelManager.Instance.AddMoney(currentValue);

        Clear();

        // play sound - drop items
    }

    public bool HasSpace(float weight)
    {
        if (currentCarryAmount + weight > carryLimit)
        {
            // Display error text
            // play sound - error
            return false;
        }

        return true;
    }
}
