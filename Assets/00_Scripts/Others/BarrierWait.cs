using UnityEngine;

public class BarrierWait : MonoBehaviour
{
    Vector3 Position;
    private void Start()
    {
        Position = transform.position;
    }

    private void Update()
    {
        if (transform.position != Position)
        {
            transform.position = Position;
        }
    }
}
