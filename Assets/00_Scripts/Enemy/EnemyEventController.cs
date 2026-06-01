using UnityEngine;

public class EnemyEventController : MonoBehaviour
{
    EnemyController enemyController;

    EnemyAnimationController enemyAnimationController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyAnimationController = GetComponentInChildren<EnemyAnimationController>();
    }

    private void OnEnable()
    {
        enemyController.OnAttacked += enemyAnimationController.SetAttack;
        enemyController.OnWalked += enemyAnimationController.SetWalk;
        enemyController.OnFind += enemyAnimationController.SetFind;
        enemyController.OnDeath += enemyAnimationController.SetDeath;
    }

    private void OnDisable()
    {
        enemyController.OnAttacked -= enemyAnimationController.SetAttack;
        enemyController.OnWalked -= enemyAnimationController.SetWalk;
        enemyController.OnFind -= enemyAnimationController.SetFind;
        enemyController.OnDeath -= enemyAnimationController.SetDeath;
    }
}
