using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text noSpaceText;
    [SerializeField] private Color outlineColor;

    [SerializeField, Space] private int weight;
    [SerializeField] private int value;

    [SerializeField, Space] private bool canInteract = true;
    [SerializeField] private bool isFragile = false;

    private float canvasOffsetY;

    private CollisionCheck collisionCheck;
    private Outline outline;

    private void Awake()
    {
        //modelTransform = GetComponentInChildren<MeshRenderer>().transform;

        if (modelTransform.TryGetComponent(out Outline modelOutline))
        {
            outline = modelOutline;
        }
        else
        {
            outline = modelTransform.AddComponent<Outline>();
        }

        if (modelTransform.TryGetComponent(out CollisionCheck modelCollision))
        {
            collisionCheck = modelCollision;
        }
        else
        {
            collisionCheck = modelTransform.AddComponent<CollisionCheck>();
        }

        if (!modelTransform.TryGetComponent(out Rigidbody rb))
        {
            modelTransform.AddComponent<Rigidbody>();
        }

        outline.OutlineColor = outlineColor;
        outline.enabled = false;

        collisionCheck.collisionLayer = LayerMask.GetMask("Ground");
        modelTransform.gameObject.layer = LayerMask.NameToLayer("Interactable");

        weightText.text = $"{weight} kg / {value} $";
        weightText.enabled = false;
        noSpaceText.enabled = false;

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
        // play sound break

        Destroy(gameObject);
    }

    public void Focus()
    {
        outline.enabled = true;
        weightText.enabled = true;

        if (!PlayerManager.Inventory.HasSpace(weight))
        {
            noSpaceText.enabled = true;
            return;
        }
    }

    public void LoseFocus()
    {
        outline.enabled = false;
        weightText.enabled = false;
        noSpaceText.enabled = false;
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

        // play sound - pickup

        Destroy(gameObject);
    }

    public void SetCanInteract(bool value) => canInteract = value;
}
