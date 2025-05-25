using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private TMP_Text inventorySpaceText;
    [SerializeField] private TMP_Text playerCanvasText;
    [SerializeField] private Transform guidingArrowTransform;
    [SerializeField] private int weightCarryLimit;

    private int currentWeight;
    private int currentValue;

    private bool firstItemWasAdded = false;

    private Transform vanTransform;
    private Animator playerCanvasTextAnim;

    private void Awake()
    {
        // Initialize inventory
        Clear();
        SetInventorySpaceText();

        // Initialize player text
        playerCanvasText.alpha = 0;
        playerCanvasTextAnim = playerCanvasText.GetComponent<Animator>();

        vanTransform = FindFirstObjectByType<Van>().transform;
    }

    private void Update()
    {
        // Update the arrow pointing towards the van
        if (currentWeight >= weightCarryLimit)
        {
            guidingArrowTransform.LookAt(vanTransform);
            guidingArrowTransform.eulerAngles = new Vector3(0, guidingArrowTransform.eulerAngles.y, 0);
        }      
    }

    private void Clear()
    {
        currentWeight = 0;
        currentValue = 0;
    }

    private void SetInventorySpaceText()
    {
        inventorySpaceText.text = $"{currentWeight}/{weightCarryLimit} kg";
    }

    private void PlayTextAnimation()
    {
        playerCanvasTextAnim.Play("TextIdle", -1, 0f);
        playerCanvasTextAnim.Play("TextPopup", -1, 0f);
    }

    public void AddItem(int weight, int value)
    {
        currentWeight += weight;
        currentValue += value;

        SetInventorySpaceText();

        // Set text settings and display it
        playerCanvasText.text = $"+ {weight} kg";
        playerCanvasText.color = Color.yellow;
        PlayTextAnimation();

        // Start the countdown if it's the first item
        if (!firstItemWasAdded)
        {
            LevelManager.Instance.StartCountdown();
            firstItemWasAdded = true;
        }

        // Activate an arrow pointing towards the van when weight limit is reached
        if (currentWeight >= weightCarryLimit)
        {
            guidingArrowTransform.gameObject.SetActive(true);
        }
    }

    public void DropItemsToVan()
    {
        if (currentValue <= 0) { return; }

        LevelManager.Instance.AddMoney(currentValue);
        AudioPlayer.Instance.PlaySoundEffect("money");

        // Set text settings and display it
        playerCanvasText.text = $"+{currentValue} $";
        playerCanvasText.color = Color.green;  
        PlayTextAnimation();

        // Reset inventory stats
        Clear();
        SetInventorySpaceText();
        guidingArrowTransform.gameObject.SetActive(false);
    }

    public bool HasSpace(int weight)
    {
        if (currentWeight + weight > weightCarryLimit)
        {
            return false;
        }

        return true;
    }
}
