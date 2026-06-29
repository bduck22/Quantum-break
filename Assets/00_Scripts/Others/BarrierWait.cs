using UnityEngine;

public class BarrierWait : MonoBehaviour
{
    Quaternion Rotation;
    Rigidbody rb;

    private void Start()
    {
        Rotation = transform.rotation;
        
    }

    private void Update()
    {
        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
            if (rb == null)
            {
                this.enabled = false;
            }
            else if(!rb.isKinematic)
            {
                this.enabled= false;
            }
        }

        if (transform.rotation != Rotation)
        {
            transform.rotation = Rotation;
        }
    }
}
