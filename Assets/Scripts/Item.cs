using UnityEngine;

public class Item : MonoBehaviour
{
    public float Weight;
    public float Value;

    public bool CanInteract = true;

    public void Focus()
    {
        Debug.Log("Looking at item " + gameObject.name);
        // activate outline and stats
    }

    public void LoseFocus()
    {
        Debug.Log("Stopped looking at item " + gameObject.name);
        // deactivate outline and stats
    }

    public void Pickup()
    {
        // play sound - pickup
        Destroy(gameObject);
    }
}
