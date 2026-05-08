using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public Transform CameraRoot;

    public float XRotation;

    public float MinRotation;
    public float MaxRotation;

    float deltaYaw;

    public void Rotate(float Sensitivity, Vector2 Input)
    {
        float mouseX = Input.x * Sensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.y * Sensitivity * Time.unscaledDeltaTime;

        transform.Rotate(Vector3.up * mouseX);

        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -80f, 80f);
        CameraRoot.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
    }

    private void Update()
    {
        if(deltaYaw != 0)
        {
            transform.Rotate(Vector3.up * (deltaYaw * Time.deltaTime));
            if(deltaYaw > 0)
            {
                deltaYaw -= Time.deltaTime;
            }
            else if(deltaYaw < 0)
            {
                deltaYaw += Time.deltaTime;
            }
            if(Mathf.Abs(deltaYaw) < 0.01f)
            {
                deltaYaw = 0;
            }
        }
    }

    public void WallRotate(float deltaYaw)
    {
        this.deltaYaw = deltaYaw*3.015f;
    }

    public void WallRotateStop()
    {
        deltaYaw = 0;
    }
}
