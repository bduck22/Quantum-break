using UnityEngine;

public class UISizeController : MonoBehaviour
{
    public Transform CanvasLeft;
    public Transform CanvasRight;

    public void CanvasRefresh(float fov)
    {
        float value = 0.01f + 0.002f * ((fov - 70) * 0.05f);
        Vector3 scale = new Vector3(value, value, value);

        CanvasLeft.transform.localScale = scale;
        CanvasRight.transform.localScale = scale;
    }
}
