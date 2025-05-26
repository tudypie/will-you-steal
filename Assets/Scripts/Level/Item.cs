using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text noSpaceText;
    [SerializeField] private GameObject interactIcon;

    [Header("Settings")]
    [SerializeField] private int weight;
    [SerializeField] private int value;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool isFragile = false;
    [SerializeField] private bool isRbKinematic = false;
    [SerializeField] private bool isOnStand = true;
    [SerializeField] private Color outlineColor;

    private float canvasOffsetY;

    private CollisionCheck collisionCheck;
    private Outline outline;

    private void Awake()
    {
        // Add missing components to the model:

        // Rigidbody
        if (!modelTransform.TryGetComponent(out Rigidbody rb))
        {
            modelTransform.gameObject.AddComponent<Rigidbody>();
        }

        // Outline
        if (!modelTransform.TryGetComponent(out Outline modelOutline))
        {
            modelTransform.gameObject.AddComponent<Outline>();
        }

        // Collision Check
        if (!modelTransform.TryGetComponent(out CollisionCheck col))
        {
            modelTransform.gameObject.AddComponent<CollisionCheck>();
        }

        // Setup Rigidbody
        modelTransform.GetComponent<Rigidbody>().isKinematic = isRbKinematic;

        // Setup outline settings
        outline = modelTransform.GetComponent<Outline>();
        outlineColor = isFragile ? Color.yellow : Color.green;
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = 1.2f;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.enabled = false;

        // Setup layers
        collisionCheck = modelTransform.GetComponent<CollisionCheck>();
        collisionCheck.collisionLayer = LayerMask.GetMask("Ground");
        modelTransform.gameObject.layer = LayerMask.NameToLayer("Interactable");

        // Initialize UI
        statsText.text = $"{weight} kg / {value} $";
        statsText.enabled = false;
        noSpaceText.enabled = false;
        interactIcon.SetActive(false);
        canvasOffsetY = canvasTransform.position.y - modelTransform.position.y;
    }

    private void OnEnable()
    {
        if (isOnStand) { collisionCheck.OnCollision += OnTouchGround; }
    }

    private void OnDisable()
    {
        if (isOnStand) { collisionCheck.OnCollision -= OnTouchGround; }
    }

    private void Update()
    {
        // Update the canvas to always be above the model
        canvasTransform.position = new Vector3(modelTransform.position.x, modelTransform.position.y + canvasOffsetY, modelTransform.position.z);
    }

    private void OnTouchGround()
    {
        Debug.Log(gameObject.name + " touched ground");

        if (isFragile)
        {
            AudioPlayer.Instance.PlaySoundEffect("break");
            DisableItem();
        }

        if (!LevelManager.Instance.CountdownHasStarted)
        {
            LevelManager.Instance.StartCountdown();
        }
    }

    private void DisableItem()
    {
        canvasTransform.gameObject.SetActive(false);
        Destroy(modelTransform.gameObject);
        enabled = false;
    }

    public void Focus()
    {
        outline.enabled = true;
        statsText.enabled = true;
        interactIcon.SetActive(true);

        if (!PlayerManager.Inventory.HasSpace(weight))
        {
            noSpaceText.enabled = true;
            return;
        }
    }

    public void LoseFocus()
    {
        interactIcon.SetActive(false);
        /*outline.enabled = false;
        statsText.enabled = false;
        noSpaceText.enabled = false;*/
    }

    public void Interact()
    {
        if (!canInteract) { return; }

        if (!PlayerManager.Inventory.HasSpace(weight)) { return; }

        if (isFragile)
        {
            canInteract = false;
            PlayerManager.Wrapping.StartWrapping(this);
        }
        else
        {
            Pickup();
        }
    }

    public void Pickup()
    {
        PlayerManager.Inventory.AddItem(weight, value);

        AudioPlayer.Instance.PlaySoundEffect("pickup");

        DisableItem();
    }

    public void SetCanInteract(bool value) => canInteract = value;
}
