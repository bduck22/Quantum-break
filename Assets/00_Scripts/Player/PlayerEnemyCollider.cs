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
        Vector3 dir = transform.position - other.transform.position;
        cc.Move(dir.normalized * HitPower * Time.deltaTime);
    }
}
