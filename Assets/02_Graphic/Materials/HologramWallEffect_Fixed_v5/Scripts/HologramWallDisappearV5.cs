using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class HologramWallDisappearV5 : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Collider targetCollider;

    [Header("Timing")]
    [SerializeField] private float disappearDuration = 1.15f;
    [SerializeField] private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField, Range(0f, 1f)] private float colliderOffAt = 0.62f;

    [Header("Optional Effect")]
    [SerializeField] private ParticleSystem disappearParticle;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip disappearSound;

    [Header("End State")]
    [SerializeField] private bool disableRendererWhenFinished = true;
    [SerializeField] private bool disableGameObjectWhenFinished = false;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine routine;

    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    public bool IsOpen;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetCollider == null)
        {
            targetCollider = GetComponent<Collider>();
        }

        propertyBlock = new MaterialPropertyBlock();
        SetDissolveAmount(0f);
    }

    [ContextMenu("Play Disappear")]
    public void PlayDisappear()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(DisappearRoutine());
    }

    [ContextMenu("Reset Wall")]
    public void ResetWall()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }

        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }

        gameObject.SetActive(true);
        SetDissolveAmount(0f);
    }

    private IEnumerator DisappearRoutine()
    {
        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }

        if (disappearParticle != null)
        {
            disappearParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            disappearParticle.Play(true);
        }

        if (audioSource != null && disappearSound != null)
        {
            audioSource.PlayOneShot(disappearSound);
        }

        float elapsed = 0f;
        bool colliderDisabled = false;

        while (elapsed < disappearDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(disappearDuration, 0.0001f));
            float dissolve = dissolveCurve.Evaluate(t);

            SetDissolveAmount(dissolve);

            if (!colliderDisabled && dissolve >= colliderOffAt)
            {
                colliderDisabled = true;

                if (targetCollider != null)
                {
                    targetCollider.enabled = false;
                }
            }

            yield return null;
        }

        SetDissolveAmount(1f);

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        if (disableRendererWhenFinished && targetRenderer != null)
        {
            targetRenderer.enabled = false;
        }

        if (disableGameObjectWhenFinished)
        {
            gameObject.SetActive(false);
        }

        routine = null;
        TutorialManager.Instance.NextInfo();
    }

    private void SetDissolveAmount(float value)
    {
        if (targetRenderer == null)
        {
            return;
        }

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(DissolveAmountID, value);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    bool one=false;

    private void OnTriggerEnter(Collider other)
    {
        if(!IsOpen||one) return;

        if (other.gameObject.layer == 11)
        {
            one = true;
            PlayDisappear();
        }
    }
}
