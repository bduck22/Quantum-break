using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class PlayerAttackHitChecker : MonoBehaviour
{
    PlayerController playerController;
    public Transform Head;
    public Vector3 HeadPivot = new Vector3(0,0.3f,0);
    public Transform camera;
    
    int layer;

    public bool isparried = false;

    public bool Killed;

    public LayerMask Layer;

    public SoundRandomPlayer soundRandomPlayer;

    private void OnEnable()
    {
        Killed = false;
    }

    private void Awake()
    {
        layer = LayerMask.GetMask("Bullet");// | LayerMask.GetMask("Enemy");
        camera = Camera.main.transform;

        playerController = transform.root.GetComponent<PlayerController>();

        playerController.OnAttack += InitParring;
    }

    public void InitParring()
    {
        isparried = false;
    }

    public SampleKnifeSlicer Slicer;

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
                    collider.transform.parent.GetComponent<BulletController>().Parring(camera);//Head.position+HeadPivot
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

    public SlowMotionAudioFilter Filter;

    public Vector3 pivot = new Vector3(0,1.5f,0);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (AttackCheck()&&!Killed)
            {
                RaycastHit hit;
                if(Physics.Raycast(Head.position, ((other.attachedRigidbody.position+pivot) - Head.position).normalized,out hit,  100, Layer))
                {
                    if (hit.transform.gameObject.layer != 8) return;

                    soundRandomPlayer.SoundPlay();
                    Filter.EnterSlowMotion();

                    EnemyController enemy = other.attachedRigidbody.GetComponent<EnemyController>();

                    enemy.Hit();

                    enemy.Animation.enabled = false;

                    CallEnemySlice(other);

                    Killed = true;
                }
            }
        }
        if (other.gameObject.layer == 10)
        {
            AttackCheck();
        }
    }

    private void OnDisable()
    {
        if (Killed)
        {
            Filter.ExitSlowMotion();
        }
    }

    [SerializeField] private Transform swordTransform;

    private async void CallEnemySlice(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return;
        }

        EnemySliceExecutor sliceExecutor = hitCollider.GetComponentInParent<EnemySliceExecutor>();

        await sliceExecutor.TrySliceBySwordYAtFixedPoint(swordTransform);
    }
}
