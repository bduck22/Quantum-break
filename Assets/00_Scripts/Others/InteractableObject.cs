using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract void Interaction();

    public abstract string GetInfo();

    public abstract bool IsInteract();
}
