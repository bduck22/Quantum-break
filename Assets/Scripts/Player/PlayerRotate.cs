using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public Transform CameraRoot;

    public float XRotation;

    public float MinRotation;
    public float MaxRotation;

    public void Rotate(float Sensitivity, Vector2 Input)
    {
        float mouseX = Input.x * Sensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.y * Sensitivity * Time.unscaledDeltaTime;

        transform.Rotate(Vector3.up * mouseX);

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -80f, 80f);
        CameraRoot.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
    }
}
