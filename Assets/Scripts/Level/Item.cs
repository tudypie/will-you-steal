using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text noSpaceText;
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private Color outlineColor;

    [SerializeField, Space] private int weight;
    [SerializeField] private int value;

    [SerializeField, Space] private bool canInteract = true;
    [SerializeField] private bool isFragile = false;
    [SerializeField] private bool isRbKinematic = false;

    private float canvasOffsetY;

    private CollisionCheck collisionCheck;
    private Outline outline;

    private void Awake()
    {
        // Add missing components to the model:

        // Rigidbody
        if (!modelTransform.TryGetComponent(out Rigidbody rb))
        {
            modelTransform.AddComponent<Rigidbody>();
        }

        // Outline
        if (!modelTransform.TryGetComponent(out Outline modelOutline))
        {
            modelTransform.AddComponent<Outline>();
            outline = modelOutline;
        }

        // Collision Check
        if (!modelTransform.TryGetComponent(out CollisionCheck col))
        {
            modelTransform.AddComponent<CollisionCheck>();
            collisionCheck = col;
        }

        // Setup Rigidbody
        modelTransform.GetComponent<Rigidbody>().isKinematic = isRbKinematic;

        // Setup outline settings
        if (outline == null) { outline = modelTransform.GetComponent<Outline>(); }

        outlineColor = isFragile ? Color.yellow : Color.green;
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = 1.2f;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.enabled = false;

        // Setup layers
        if (collisionCheck == null) { collisionCheck = modelTransform.GetComponent<CollisionCheck>(); }

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
        if (isFragile)
        {
            collisionCheck.OnCollision += OnTouchGround;
        }
    }

    private void OnDisable()
    {
        if (isFragile)
        {
            collisionCheck.OnCollision -= OnTouchGround;
        }
    }

    private void Update()
    {
        canvasTransform.position = new Vector3(modelTransform.position.x, modelTransform.position.y + canvasOffsetY, modelTransform.position.z);
    }

    private void OnTouchGround()
    {   
        AudioPlayer.Instance.PlaySoundEffect("break");
        DisableItem();
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
