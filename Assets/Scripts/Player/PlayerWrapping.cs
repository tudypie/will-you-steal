using UnityEngine;
using UnityEngine.UI;

public class PlayerWrapping : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject wrappingPanel;
    [SerializeField] private RectTransform barTransform;
    [SerializeField] private RectTransform lineTransform;
    [SerializeField] private RectTransform spotTransform;
    [SerializeField] private Image wrapProgressFillBar;
    [SerializeField] private AudioSource wrappingSound;

    [Header("Settings")]
    [SerializeField] private float progressPerSkillCheck;
    [SerializeField] private float lineSpeed;
    [SerializeField] private float spotSizeDecrease;
    [SerializeField] private float missDelay;

    private float currentMissDelay, wrapProgress, barLength, initialSpotLength;
    private int lineDirection;

    private bool lineIsMovingForward;
    private bool isWrapping;

    private Vector2 barPositionRange;
    private Vector2 spotPositionRange;

    private Item currentWrappingItem;

    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

        // Initialize variables
        currentMissDelay = 0;
        isWrapping = false;

        barLength = barTransform.rect.width;
        initialSpotLength = spotTransform.rect.width;

        barPositionRange = new Vector2(-barLength / 2, barLength / 2);
    }

    private void Update()
    {
        if (!isWrapping) { return; }

        // Stop interacting if player presses ESC or Q
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            currentWrappingItem.SetCanInteract(true);
            StopWrapping();
        }

        // Stop interacting if item broke while interacting (fell on ground)
        if (currentWrappingItem == null)
        {
            StopWrapping();
            return;
        }

        // Wait a delay if player misssed skill check
        if (currentMissDelay > 0)
        {
            currentMissDelay -= Time.deltaTime;
            return;
        }

        lineTransform.gameObject.SetActive(true);
        spotTransform.gameObject.SetActive(true);

        // Check if vertical line reached min/max position and change movement direction
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

        // Calculate next position of vertical line
        float newLineX = lineTransform.localPosition.x + lineSpeed * lineDirection * Time.deltaTime;
        lineTransform.localPosition = new Vector2(newLineX, lineTransform.localPosition.y);

        // Wait for player input
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        // If horizontal line is in spot range increase progress
        if (newLineX >= spotPositionRange.x && newLineX <= spotPositionRange.y)
        {
            wrapProgress += progressPerSkillCheck;
            wrapProgressFillBar.fillAmount = wrapProgress / 100;

            // Decrease spot width to make it harder
            float newSpotWidth = spotTransform.rect.width - spotSizeDecrease;
            spotTransform.sizeDelta = new Vector2(newSpotWidth, spotTransform.sizeDelta.y);

            AudioPlayer.Instance.PlaySoundEffect("correct");

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

            AudioPlayer.Instance.PlaySoundEffect("wrong");
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

        // Initialize wrapping UI
        wrapProgress = 0;
        wrapProgressFillBar.fillAmount = 0;
        wrappingPanel.SetActive(true);

        lineDirection = 1;
        lineIsMovingForward = true;

        spotTransform.sizeDelta = new Vector2(initialSpotLength, spotTransform.sizeDelta.y);
        GenerateRandomSpotPosition();

        // Set a small delay to prevent stacked input
        currentMissDelay = 0.01f;

        wrappingSound.Play();

        isWrapping = true;
    }

    public void StopWrapping()
    {
        currentWrappingItem = null;

        movement.enabled = true;

        wrappingPanel.SetActive(false);
        wrapProgress = 0;
        wrapProgressFillBar.fillAmount = 0;

        wrappingSound.Stop();

        isWrapping = false;
    }
}
