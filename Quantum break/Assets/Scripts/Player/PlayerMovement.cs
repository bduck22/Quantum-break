using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void Move()
    {

    }
}
