using UnityEngine;

public class EnemyBulletHit : MonoBehaviour
{
    EnemyController controller;

    private void Start()
    {
        controller = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            if (other.CompareTag("Bullet"))
            {
                other.transform.parent.GetComponent<BulletController>().OnFalse();
                SpawnManagers.Instance.Particle.SpawnParticle(Particle_Type.EnemyHit, transform.position, Quaternion.identity).Play();
                controller.Hit();
            }
        }
    }
}