using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private TMP_Text inventorySpaceText;
    [SerializeField] private TMP_Text playerCanvasText;

    private Animator playerCanvasTextAnim;

    [SerializeField] private Transform guidingArrowTransform;

    [SerializeField] private int carryLimit;

    private int currentCarryAmount;
    private int currentValue;

    private bool firstItemWasAdded = false;

    private Transform vanTransform;

    private void Awake()
    {
        Clear();
        UpdateUI();

        playerCanvasText.alpha = 0;
        playerCanvasTextAnim = playerCanvasText.GetComponent<Animator>();
    }

    private void Start()
    {
        vanTransform = FindFirstObjectByType<Van>().transform;
    }

    private void Update()
    {
        if (currentCarryAmount >= carryLimit)
        {
            guidingArrowTransform.LookAt(vanTransform);
            guidingArrowTransform.eulerAngles = new Vector3(0, guidingArrowTransform.eulerAngles.y, 0);
        }      
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

    private void PlayTextAnimation()
    {
        playerCanvasTextAnim.Play("TextIdle", -1, 0f);
        playerCanvasTextAnim.Play("TextPopup", -1, 0f);
    }

    public void AddItem(int weight, int value)
    {
        currentCarryAmount += weight;
        currentValue += value;

        UpdateUI();

        playerCanvasText.text = $"+ {weight} kg";
        playerCanvasText.color = Color.yellow;
        PlayTextAnimation();

        if (!firstItemWasAdded)
        {
            LevelManager.Instance.StartCountdown();
            firstItemWasAdded = true;
        }

        if (currentCarryAmount >= carryLimit)
        {
            guidingArrowTransform.gameObject.SetActive(true);
        }
    }

    public void DropItemsToVan()
    {
        if (currentValue <= 0) { return; }

        LevelManager.Instance.AddMoney(currentValue);

        Clear();

        guidingArrowTransform.gameObject.SetActive(false);

        playerCanvasText.text = "$$$";
        playerCanvasText.color = Color.green;  
        PlayTextAnimation();

        // play sound - drop items (cash)
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
