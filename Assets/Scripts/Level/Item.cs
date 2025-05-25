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
    [SerializeField] private bool kinematicRb = false;

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
            modelTransform.GetComponent<Rigidbody>().isKinematic = kinematicRb;
        }

        if (isFragile)
        {
            outlineColor = Color.yellow;
        }
        else
        {
            outlineColor = Color.green;
        }

        outline.OutlineColor = outlineColor;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 1.2f;
        outline.enabled = false;

        collisionCheck.collisionLayer = LayerMask.GetMask("Ground");
        modelTransform.gameObject.layer = LayerMask.NameToLayer("Interactable");

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
        if (modelTransform == null) { return; }

        interactIcon.SetActive(false);

        //outline.enabled = false;
        //statsText.enabled = false;
        //noSpaceText.enabled = false;
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
