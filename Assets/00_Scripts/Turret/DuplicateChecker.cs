using UnityEngine;

public class DuplicateChecker : MonoBehaviour
{
    [SerializeField] bool duplip;
    public bool IsDuplip()
    {
        return duplip;
    }

    public void OnDisable()
    {
        duplip = false;
    }

    private void OnTriggerStay(Collider other)
    {
        duplip = true;
    }

    private void OnTriggerExit(Collider other)
    {
        duplip = false;
    }
}
