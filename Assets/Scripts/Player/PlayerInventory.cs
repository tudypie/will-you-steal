using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private TMP_Text inventorySpaceText;

    [SerializeField] private int carryLimit;

    private int currentCarryAmount;
    private int currentValue;

    private bool firstItemWasAdded = false;

    private void Awake()
    {
        Clear();
        UpdateUI();
    }

    private void Clear()
    {
        currentCarryAmount = 0;
        currentValue = 0;
    }

    private void UpdateUI()
    {
        inventorySpaceText.text = $"{currentCarryAmount}/{carryLimit} kg";
    }

    public void AddItem(int weight, int value)
    {
        currentCarryAmount += weight;
        currentValue += value;

        UpdateUI();

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

    public bool HasSpace(int weight)
    {
        if (currentCarryAmount + weight > carryLimit)
        {
            // display error text
            // play sound - error
            return false;
        }

        return true;
    }
}
