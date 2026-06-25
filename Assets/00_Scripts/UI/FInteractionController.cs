using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FInteractionController : MonoBehaviour
{
    public Image Gauge;

    public TextMeshProUGUI Name;

    public GameObject FUI;

    Image NonInteract;

    private void Awake()
    {
        NonInteract = GetComponent<Image>();
    }

    public void SetText(string Text)
    {
        Name.text = Text;
    }

    public void SetGauge(float value)
    {
        if (value == 0)
        {
            Gauge.fillAmount = 1;
        }
        else
        {
            Gauge.fillAmount = value;
        }
    }

    public void SetActiveInteraction(bool interactionmode)
    {
        FUI.SetActive(interactionmode);
        NonInteract.enabled = !interactionmode;
        if (!interactionmode)
        {
            Name.text = "";
        }
    }
}
