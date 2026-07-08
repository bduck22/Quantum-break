using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public bool End;

    public abstract void Interaction();

    public abstract string GetInfo();

    public abstract bool IsInteract();
}
