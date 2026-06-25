using UnityEngine;

[DefaultExecutionOrder(-10)]
public class UIUpdateManager : MonoBehaviour
{
    public static UIUpdateManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public UIController UIController;

    public FInteractionController FController;

    public CanvasGroup Canvas;

    public void OffCanvas()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void OnCanvas()
    {
        Canvas.alpha = 0;
        Canvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Canvas.gameObject.activeSelf&&Canvas.alpha !=1)
        {
            Canvas.alpha += Time.deltaTime*1.5f;
            if(Canvas.alpha >= 1)
            {
                Canvas.alpha = 1;
            }
        }
    }
}
