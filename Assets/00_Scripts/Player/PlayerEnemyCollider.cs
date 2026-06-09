using UnityEngine;

public class PlayerEnemyCollider : MonoBehaviour
{
    CharacterController cc;

    public float HitPower;

    private void Start()
    {
        cc = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            Vector3 dir = transform.position - other.transform.position;
            cc.Move(dir.normalized * HitPower * Time.deltaTime);
        }
    }
}
