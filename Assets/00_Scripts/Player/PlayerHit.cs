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
        if (other.CompareTag("Bullet"))
        {
            if (!controller.Invincibility)
            {
                SpawnManagers.Instance.Particle.SpawnParticle(Particle_Type.Playerhit, other.transform.position, Quaternion.identity).Play();
                other.transform.parent.GetComponent<BulletController>().OnFalse();
                controller.OnHited();
            }
        }
    }
}
