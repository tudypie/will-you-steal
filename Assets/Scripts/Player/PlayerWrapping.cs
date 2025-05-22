using UnityEngine;
using UnityEngine.UI;

public class PlayerWrapping : MonoBehaviour
{
    public bool IsWrapping = false;

    [SerializeField] private GameObject wrappingPanel;
    [SerializeField] private RectTransform barTransform;
    [SerializeField] private RectTransform lineTransform;
    [SerializeField] private RectTransform spotTransform;
    [SerializeField] private Image wrapProgressFillBar;

    [SerializeField, Space] private float progressPerSkillCheck;
    [SerializeField] private float lineSpeed;
    [SerializeField] private float spotSizeDecrease;
    [SerializeField] private float missDelay;

    private float currentMissDelay = 0;

    private float wrapProgress;
    private float barLength;
    private float initialSpotLength;
    private float lineDirection;
    private bool lineIsMovingForward;

    private Vector2 barPositionRange;
    private Vector2 spotPositionRange;

    private Item currentWrappingItem;

    private PlayerMovement movement;
    private PlayerInventory inventory;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        inventory = GetComponent<PlayerInventory>();

        barLength = barTransform.rect.width;
        initialSpotLength = spotTransform.rect.width;

        barPositionRange = new Vector2(-barLength / 2, barLength / 2);
    }

    private void Update()
    {
        if (!IsWrapping) { return; }

        if (currentWrappingItem == null)
        {
            StopWrapping();
            return;
        }

        if (currentMissDelay > 0)
        {
            currentMissDelay -= Time.deltaTime;
            return;
        }

        lineTransform.gameObject.SetActive(true);
        spotTransform.gameObject.SetActive(true);

        if (lineTransform.localPosition.x >= barPositionRange.y && lineIsMovingForward)
        {
            lineDirection = -1;
            lineIsMovingForward = false;
        }
        else if (lineTransform.localPosition.x <= barPositionRange.x && !lineIsMovingForward)
        {
            lineDirection = 1;
            lineIsMovingForward = true;
        }

        float newLineX = lineTransform.localPosition.x + lineSpeed * lineDirection * Time.deltaTime;
        lineTransform.localPosition = new Vector2(newLineX, lineTransform.localPosition.y);

        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        if (newLineX >= spotPositionRange.x && newLineX <= spotPositionRange.y)
        {
            wrapProgress += progressPerSkillCheck;
            wrapProgressFillBar.fillAmount = wrapProgress / 100;

            float newSpotWidth = spotTransform.rect.width - spotSizeDecrease;
            spotTransform.sizeDelta = new Vector2(newSpotWidth, spotTransform.sizeDelta.y);

            // play sound - correct

            if (wrapProgress >= 100)
            {
                currentWrappingItem.Pickup();
                StopWrapping();
            }
        }
        else
        {
            currentMissDelay = missDelay;

            lineTransform.gameObject.SetActive(false);
            spotTransform.gameObject.SetActive(false);

            lineTransform.localPosition = new Vector2(barPositionRange.x, lineTransform.localPosition.y);

            // play sound - miss
        }

        GenerateRandomSpotPosition();
        
    }

    private void GenerateRandomSpotPosition()
    {
        float randomX = Random.Range(barPositionRange.x, barPositionRange.y);

        spotTransform.localPosition = new Vector2(randomX, spotTransform.localPosition.y);

        spotPositionRange = new Vector2(randomX - spotTransform.rect.width / 2, randomX + spotTransform.rect.width / 2);
    }

    public void StartWrapping(Item item)
    {
        currentWrappingItem = item;

        movement.enabled = false;

        wrapProgress = 0;
        wrapProgressFillBar.fillAmount = 0;
        wrappingPanel.SetActive(true);

        lineDirection = 1;
        lineIsMovingForward = true;

        spotTransform.sizeDelta = new Vector2(initialSpotLength, spotTransform.sizeDelta.y);

        GenerateRandomSpotPosition();

        currentMissDelay = 0.01f;

        // play sound - looped wrapping

        IsWrapping = true;
    }

    public void StopWrapping()
    {
        currentWrappingItem = null;

        movement.enabled = true;

        wrappingPanel.SetActive(false);
        wrapProgress = 0;
        wrapProgressFillBar.fillAmount = 0;

        // stop sound - looped wrapping

        IsWrapping = false;
    }
}
