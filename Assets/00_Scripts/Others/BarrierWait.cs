using UnityEngine;

public class BarrierWait : MonoBehaviour
{
    Quaternion Rotation;
    EnemySliceExecutor Slicer;

    EnemyController controller;

    MeshRenderer renderer;

    bool Stop;

    private void Start()
    {
        Slicer = transform.GetComponentInParent<EnemySliceExecutor>();
        controller = transform.parent.parent.GetComponent<EnemyController>();
        renderer = GetComponentInChildren<MeshRenderer>();

        Rotation = Quaternion.identity;

        controller.OnFind += Init;
        controller.OnWalked += False;
    }

    public void Init()
    {
        Stop = true;
        renderer.enabled = true;
    }

    public void False()
    {
        Stop = false;
        renderer.enabled = false;
    }

    private void Update()
    {
        if (Slicer.IsSliced || controller.IsDead)
        {
            gameObject.SetActive(false);
        }

        if (!Stop)
        {
            Rotation = transform.parent.parent.rotation;
        }
        else
        {
            if(Rotation == Quaternion.identity)
            {
                Rotation = transform.parent.parent.rotation;
            }
            transform.rotation = Rotation;
        }
    }
}
