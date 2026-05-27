using UnityEngine;

public class PlayerAttackHitChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            EnemyController enemy = other.attachedRigidbody.GetComponent<EnemyController>();

            enemy.Hit();
        }
    }
}
