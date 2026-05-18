using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGhostEffect : MonoBehaviour
{
    [SerializeField] private Material ghostMaterial;
    [SerializeField] private Camera targetCamera;

    [Header("Ghost Settings")]
    [SerializeField] private float peakIntensity = 0.55f;
    [SerializeField] private float startOffset = 0.012f;
    [SerializeField] private float duration = 1;
    [SerializeField] private Color tint = Color.white;

    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private static readonly int OffsetID = Shader.PropertyToID("_Offset");
    private static readonly int DirectionID = Shader.PropertyToID("_Direction");
    private static readonly int TintID = Shader.PropertyToID("_Tint");

    private void Start()
    {
        ResetEffect();
    }

    private void OnDisable()
    {
        ResetEffect();
    }

    public void PlayEffect()
    {
        StartCoroutine(CoPlay(-transform.forward));
    }

    private void ResetEffect()
    {
        if (ghostMaterial == null)
            return;

        ghostMaterial.SetFloat(IntensityID, 0f);
        ghostMaterial.SetFloat(OffsetID, 0f);
        ghostMaterial.SetVector(DirectionID, Vector4.zero);
        ghostMaterial.SetColor(TintID, tint);
    }

    private IEnumerator CoPlay(Vector3 worldDashDirection)
    {
        Vector2 screenDirection = GetScreenDirection(worldDashDirection);

        ghostMaterial.SetColor(TintID, tint);
        ghostMaterial.SetVector(DirectionID, new Vector4(screenDirection.x, screenDirection.y, 0f, 0f));

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float fade = duration - t;

            ghostMaterial.SetFloat(IntensityID, peakIntensity * fade);
            ghostMaterial.SetFloat(OffsetID, startOffset * fade);

            yield return null;
        }

        ResetEffect();
    }

    private Vector2 GetScreenDirection(Vector3 worldDirection)
    {
        if (targetCamera == null || worldDirection.sqrMagnitude < 0.001f)
            return Vector2.right;

        Vector3 localDir = targetCamera.transform.InverseTransformDirection(worldDirection.normalized);

        Vector2 screenDir = new Vector2(localDir.x, localDir.y);

        if (screenDir.sqrMagnitude < 0.001f)
            screenDir = Vector2.down;

        return screenDir.normalized;
    }
}
