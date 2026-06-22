using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public PlayerController controller;

    public LayerMask InteractLayer;

    RaycastHit hit;

    Transform camera;

    public float InteractionDistance;

    public InteractableObject Object;

    public float InteractTime;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        camera = Camera.main.transform;
    }

    private void Update()
    {
        if (controller.IsDead || Time.timeScale == 0 || controller.Stop) return;

        if (Physics.Raycast(camera.position, camera.forward, out hit, InteractionDistance, InteractLayer))
        {
            if(Object == null)
            {
                Object = hit.transform.GetComponent<InteractableObject>();
            }
        }
        else if(Object != null)
        {
            Object = null;
        }

        if (Object)
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null) return;

            if (!keyboard.anyKey.isPressed) return;

            if (keyboard.fKey.wasPressedThisFrame)
            {
                fpress = true;
            }

            if (keyboard.fKey.wasReleasedThisFrame)
            {
                fpress = false;
            }

            if (fpress)
            {
                time += Time.deltaTime;
                if(time >= InteractTime)
                {
                    time = 0;
                    fpress = false;
                    Object.Interaction();
                }
            }
            else
            {
                time = 0;
            }
        }
    }
    [SerializeField]float time;
    bool fpress;
}
