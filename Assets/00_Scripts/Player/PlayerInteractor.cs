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

    FInteractionController FController;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        camera = Camera.main.transform;

        FController = UIUpdateManager.Instance.FController;
    }

    private void Update()
    {
        if (controller.IsDead || Time.timeScale == 0 || controller.Stop || controller.IsHologram) return;

        if (Physics.Raycast(camera.position, camera.forward, out hit, InteractionDistance, InteractLayer))
        {
            if(Object == null)
            {
                Object = hit.transform.GetComponent<InteractableObject>();
                if (Object.IsInteract()) FController.SetActiveInteraction(true);
            }
        }
        else if(Object != null)
        {
            Object = null;
            FController.SetActiveInteraction(false);
        }

        if (Object&& Object.IsInteract())
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null) return;

            FController.SetText(Object.GetInfo());

            if (!keyboard.anyKey.isPressed && !fpress) return;

            if (keyboard.fKey.wasPressedThisFrame)
            {
                fpress = true;
            }

            if (keyboard.fKey.wasReleasedThisFrame)
            {
                fpress = false;
                FController.SetGauge(0);
            }

            if (fpress)
            {
                time += Time.deltaTime;
                FController.SetGauge(time / InteractTime);
                if (time >= InteractTime)
                {
                    time = 0;
                    fpress = false;
                    Object.Interaction();
                }
            }
            else
            {
                FController.SetGauge(0);
                time = 0;
            }
        }
    }
    [SerializeField]float time;
    bool fpress;
}
