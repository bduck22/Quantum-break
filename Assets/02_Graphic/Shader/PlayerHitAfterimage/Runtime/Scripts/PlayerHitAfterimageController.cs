using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IWantGoHome.ScreenEffects
{
    [Serializable]
    public sealed class PlayerHitAfterimageV7Profile
    {
        [Header("Timing")]
        [Tooltip("Real-time duration. Uses Time.unscaledDeltaTime, so timeScale does not affect it.")]
        [Range(0.1f, 3.0f)] public float duration = 1.25f;

        [Header("Afterimage")]
        [Range(0f, 2f)] public float afterimageIntensity = 1.35f;
        [Tooltip("Keep this very small. This effect is now designed as an aligned fullscreen echo, not a far displaced ghost.")]
        [Range(0f, 0.02f)] public float afterimageOffset = 0.0018f;
        [Range(0f, 0.02f)] public float rgbSplit = 0.0014f;
        [Range(0f, 0.01f)] public float zoomAmount = 0.00010f;
        [Range(0f, 2f)] public float centerGlowIntensity = 0.0f;
        [Range(0f, 1f)] public float vignetteIntensity = 0.02f;


        [Header("Pivot Randomization")]
        [Tooltip("Random pivot offset per hit. Kept intentionally tiny so the ghost stays close to the current screen.")]
        [Range(0f, 0.02f)] public float pivotRandomRangeX = 0.0f;
        [Range(0f, 0.02f)] public float pivotRandomRangeY = 0.0f;


        [Header("Random Side Offset")]
        [Tooltip("Minimum distance of the afterimage from the live screen. Kept close.")]
        [Range(0f, 0.01f)] public float randomShiftMin = 0.0008f;
        [Tooltip("Maximum distance of the afterimage from the live screen. Still close, but noticeably visible.")]
        [Range(0f, 0.02f)] public float randomShiftMax = 0.0025f;
        [Tooltip("Additional sideways spread for nearby echo layers.")]
        [Range(0f, 0.01f)] public float secondarySpread = 0.00045f;

        [Header("Snapshot Orientation")]
        [Tooltip("Turn this off if the captured afterimage appears vertically flipped in your project.")]
        public bool flipSnapshotY = true;
        [Tooltip("Usually keep this off. Turn this on only if the captured afterimage appears horizontally mirrored.")]
        public bool flipSnapshotX = false;

        [Header("Capture")]
        [Tooltip("1 = full screen. Full resolution helps the aligned echo match the current screen more accurately.")]
        [Range(0.25f, 1f)] public float captureResolutionScale = 1.0f;
    }

    public sealed class PlayerHitAfterimageController : MonoBehaviour
    {
        public static PlayerHitAfterimageController Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Material afterimageMaterial;

        [Header("Profile")]
        [SerializeField] private PlayerHitAfterimageV7Profile profile = new PlayerHitAfterimageV7Profile();

        [Header("Options")]
        [SerializeField] private bool makeMaterialInstance = false;
        [SerializeField] private bool dontDestroyOnLoad = false;
        [SerializeField] private bool hideOnAwake = true;

        [Header("Events")]
        public UnityEvent OnHitAfterimageStarted;
        public UnityEvent OnHitAfterimageFinished;

        private Material runtimeMaterial;
        private RenderTexture snapshotTexture;
        private Coroutine routine;
        private float manualTime;
        private int currentWidth;
        private int currentHeight;
        private Vector2 currentPivot = new Vector2(0.5f, 0.5f);
        private Vector2 currentShift = Vector2.zero;

        private static readonly int ModeID = Shader.PropertyToID("_Mode");
        private static readonly int ProgressID = Shader.PropertyToID("_Progress");
        private static readonly int ManualTimeID = Shader.PropertyToID("_ManualTime");
        private static readonly int AspectID = Shader.PropertyToID("_Aspect");
        private static readonly int SnapshotTexID = Shader.PropertyToID("_SnapshotTex");
        private static readonly int AfterimageIntensityID = Shader.PropertyToID("_AfterimageIntensity");
        private static readonly int AfterimageOffsetID = Shader.PropertyToID("_AfterimageOffset");
        private static readonly int RGBSplitID = Shader.PropertyToID("_RGBSplit");
        private static readonly int ZoomAmountID = Shader.PropertyToID("_ZoomAmount");
        private static readonly int CenterGlowIntensityID = Shader.PropertyToID("_CenterGlowIntensity");
        private static readonly int VignetteIntensityID = Shader.PropertyToID("_VignetteIntensity");
        private static readonly int SnapshotFlipXID = Shader.PropertyToID("_SnapshotFlipX");
        private static readonly int SnapshotFlipYID = Shader.PropertyToID("_SnapshotFlipY");
        private static readonly int PivotID = Shader.PropertyToID("_Pivot");
        private static readonly int RandomShiftID = Shader.PropertyToID("_RandomShift");
        private static readonly int SecondarySpreadID = Shader.PropertyToID("_SecondarySpread");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Duplicate {nameof(PlayerHitAfterimageController)} found. The first instance remains active.", this);
            }
            else
            {
                Instance = this;
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            PrepareMaterial();
            ApplyProfile();

            if (hideOnAwake)
            {
                HideImmediate();
            }
        }

        private void Update()
        {
            if (runtimeMaterial == null) return;
            manualTime += Time.unscaledDeltaTime;
            runtimeMaterial.SetFloat(ManualTimeID, manualTime);
            UpdateAspect();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;

            if (Application.isPlaying && makeMaterialInstance && runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }

            ReleaseSnapshotTexture();
        }

        public void PlayHit()
        {
            PlayHit(-1f);
        }

        public void PlayHit(float durationOverride)
        {
            PrepareMaterial();
            ApplyProfile();

            if (runtimeMaterial == null)
            {
                return;
            }

            if (routine != null)
            {
                StopCoroutine(routine);
            }

            currentPivot = new Vector2(
                0.5f + UnityEngine.Random.Range(-profile.pivotRandomRangeX, profile.pivotRandomRangeX),
                0.5f + UnityEngine.Random.Range(-profile.pivotRandomRangeY, profile.pivotRandomRangeY));

            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            float magnitude = UnityEngine.Random.Range(profile.randomShiftMin, profile.randomShiftMax);
            currentShift = dir * magnitude;

            runtimeMaterial.SetVector(PivotID, currentPivot);
            runtimeMaterial.SetVector(RandomShiftID, currentShift);
            runtimeMaterial.SetFloat(SecondarySpreadID, profile.secondarySpread);

            float duration = durationOverride > 0f ? durationOverride : profile.duration;
            routine = StartCoroutine(HitAfterimageRoutine(duration));
        }

        public void HideImmediate()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            PrepareMaterial();
            ApplyProfile();
            SetMode(0f, 1f);
        }

        private IEnumerator HitAfterimageRoutine(float duration)
        {
            SetMode(0f, 0f);
            yield return new WaitForEndOfFrame();

            EnsureSnapshotTexture();
            if (snapshotTexture == null)
            {
                yield break;
            }

            ScreenCapture.CaptureScreenshotIntoRenderTexture(snapshotTexture);
            runtimeMaterial.SetTexture(SnapshotTexID, snapshotTexture);

            OnHitAfterimageStarted?.Invoke();

            float timer = 0f;
            float safeDuration = Mathf.Max(0.01f, duration);

            while (timer < safeDuration)
            {
                timer += Time.unscaledDeltaTime;
                SetMode(1f, timer / safeDuration);
                yield return null;
            }

            SetMode(0f, 1f);
            OnHitAfterimageFinished?.Invoke();
            routine = null;
        }

        private void PrepareMaterial()
        {
            if (runtimeMaterial != null) return;

            if (afterimageMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/IWantGoHome/PlayerHitAfterimage");
                if (shader != null)
                {
                    afterimageMaterial = new Material(shader) { name = "M_PlayerHitAfterimage_Runtime" };
                }
            }

            if (afterimageMaterial == null) return;

            runtimeMaterial = makeMaterialInstance && Application.isPlaying ? new Material(afterimageMaterial) : afterimageMaterial;
            runtimeMaterial.SetTexture(SnapshotTexID, Texture2D.blackTexture);
        }

        private void ApplyProfile()
        {
            if (runtimeMaterial == null || profile == null) return;

            runtimeMaterial.SetFloat(AfterimageIntensityID, profile.afterimageIntensity);
            runtimeMaterial.SetFloat(AfterimageOffsetID, profile.afterimageOffset);
            runtimeMaterial.SetFloat(RGBSplitID, profile.rgbSplit);
            runtimeMaterial.SetFloat(ZoomAmountID, profile.zoomAmount);
            runtimeMaterial.SetFloat(CenterGlowIntensityID, profile.centerGlowIntensity);
            runtimeMaterial.SetFloat(VignetteIntensityID, profile.vignetteIntensity);
            runtimeMaterial.SetFloat(SnapshotFlipXID, profile.flipSnapshotX ? 1f : 0f);
            runtimeMaterial.SetFloat(SnapshotFlipYID, profile.flipSnapshotY ? 1f : 0f);
            runtimeMaterial.SetVector(PivotID, currentPivot);
            runtimeMaterial.SetVector(RandomShiftID, currentShift);
            runtimeMaterial.SetFloat(SecondarySpreadID, profile.secondarySpread);
            UpdateAspect();
        }

        private void SetMode(float mode, float progress)
        {
            if (runtimeMaterial == null) return;

            runtimeMaterial.SetFloat(ModeID, mode);
            runtimeMaterial.SetFloat(ProgressID, Mathf.Clamp01(progress));
            runtimeMaterial.SetFloat(ManualTimeID, manualTime);
            UpdateAspect();
        }

        private void UpdateAspect()
        {
            if (runtimeMaterial == null) return;
            float height = Mathf.Max(1, Screen.height);
            runtimeMaterial.SetFloat(AspectID, Mathf.Max(0.1f, Screen.width / height));
        }

        private void EnsureSnapshotTexture()
        {
            int width = Mathf.Max(16, Mathf.RoundToInt(Screen.width * profile.captureResolutionScale));
            int height = Mathf.Max(16, Mathf.RoundToInt(Screen.height * profile.captureResolutionScale));

            if (snapshotTexture != null && currentWidth == width && currentHeight == height)
            {
                return;
            }

            ReleaseSnapshotTexture();

            currentWidth = width;
            currentHeight = height;

            snapshotTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
            {
                name = "RT_PlayerHitAfterimageSnapshot",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
                autoGenerateMips = false
            };
            snapshotTexture.Create();
        }

        private void ReleaseSnapshotTexture()
        {
            if (snapshotTexture == null) return;

            if (Application.isPlaying)
            {
                Destroy(snapshotTexture);
            }
            else
            {
                DestroyImmediate(snapshotTexture);
            }

            snapshotTexture = null;
            currentWidth = 0;
            currentHeight = 0;
        }
    }
}
