using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    PlayerController controller;

    private void Start()
    {
        controller = transform.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Bullet"))
        {
            other.transform.parent.GetComponent<BulletController>().OnFalse();
            controller.OnHit();
        }
    }
}
