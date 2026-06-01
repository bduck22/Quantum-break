using UnityEngine;

public class PlayerAttackHitChecker : MonoBehaviour
{
    PlayerController playerController;
    public Transform Head;
    
    int layer;

    public bool isparried = false;

    private void Awake()
    {
        layer = LayerMask.GetMask("Bullet");// | LayerMask.GetMask("Enemy");
        playerController = transform.root.GetComponent<PlayerController>();

        playerController.OnAttack += InitParring;
    }

    public void InitParring()
    {
        isparried = false;
    }

    public bool AttackCheck()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.lossyScale * 0.5f, transform.rotation, layer);

        
        //bool isattacked = false;

        //EnemyController enemy = null;

        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.layer == 10)
            {
                if(collider.transform.parent.gameObject.layer == 10&&!isparried)
                {
                    collider.transform.parent.GetComponent<BulletController>().Parring(Head.position);
                    playerController.Parring();
                    isparried = true;
                    SpawnManagers.Instance.Particle.SpawnParticle(Particle_Type.SwordParring, collider.transform.position, Quaternion.identity).Play();
                }
            }
            //if(collider.gameObject.layer == 8)
            //{
            //    enemy = collider.attachedRigidbody.GetComponent<EnemyController>();
            //    isattacked = true;
            //}
        }

        return !isparried;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (AttackCheck())
            {
                EnemyController enemy = other.attachedRigidbody.GetComponent<EnemyController>();

                enemy.Hit();
            }
        }
        if (other.gameObject.layer == 10)
        {
            AttackCheck();
        }
    }
}
