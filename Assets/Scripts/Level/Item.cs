using UnityEngine;

public class Item : MonoBehaviour
{
    public int Weight;
    public int Value;

    public bool CanInteract = true;

    public void Focus()
    {
        // activate outline and stats
    }

    public void LoseFocus()
    {
        // deactivate outline and stats
    }

    public void Pickup()
    {
        // play sound - pickup
        Destroy(gameObject);
    }
}
