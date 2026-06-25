using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IWantGoHome.ScreenEffects
{
    [Serializable]
    public sealed class TVStarTransitionV12Profile
    {
        [Header("F9 Power Off")]
        [Min(0f)] public float shutdownGlitchDuration = 0.70f;
        [Min(0f)] public float shutdownStarDuration = 0.52f;
        [Min(0f)] public float holdIntroDuration = 0.12f;

        [Header("F10 Power On")]
        [Min(0f)] public float holdDissolveDuration = 0.72f;
        [Min(0f)] public float blackHoldDuration = 0.32f;
        [Min(0f)] public float powerOnFlashDuration = 6.40f;

        [Header("Current Screen Glitch")]
        [Range(0f, 3f)] public float sceneGlitchIntensity = 1.28f;
        [Range(0f, 0.16f)] public float rgbSplit = 0.038f;
        [Range(0f, 0.30f)] public float horizontalTear = 0.145f;
        [Range(0f, 0.12f)] public float waveDistortion = 0.020f;
        [Range(0f, 0.08f)] public float fineNoise = 0.007f;

        [Header("Held Dark Glitch Screen")]
        [Range(0f, 3f)] public float holdBurstIntensity = 1.00f;
        [Range(0.1f, 5f)] public float holdLineDensity = 2.60f;
        [Range(0f, 1f)] public float holdVerticalSpikeIntensity = 0.04f;

        [Header("Power-Off Star")]
        [Range(0.0005f, 0.04f)] public float starEdgeSoftness = 0.0030f;
        [Range(1f, 48f)] public float starSharpness = 22f;
        [Range(0.5f, 8f)] public float starHorizontalReach = 2.85f;
        [Range(0.1f, 3f)] public float starVerticalReach = 0.52f;
        [Range(0.001f, 0.40f)] public float starHorizontalThickness = 0.090f;
        [Range(0.001f, 0.40f)] public float starVerticalThickness = 0.030f;
        [Range(0.0001f, 0.02f)] public float starTipWidth = 0.0010f;
        [Range(0f, 5f)] public float starIntensity = 2.25f;
        [Range(0f, 3f)] public float starGlowIntensity = 0.16f;

        [Header("Power On Flash / Afterimage")]
        [Range(0f, 8f)] public float flashIntensity = 3.65f;
        [Range(0f, 2f)] public float afterimageIntensity = 1.70f;
        [Range(0f, 0.20f)] public float afterimageOffset = 0.090f;
    }

    public sealed class TVStarTransitionController : MonoBehaviour
    {
        public static TVStarTransitionController Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private Material transitionMaterial;
        [SerializeField] private CanvasGroup resultUIGroup;

        [Header("Profile")]
        [SerializeField] private TVStarTransitionV12Profile profile = new TVStarTransitionV12Profile();

        [Header("Options")]
        [SerializeField] private bool makeMaterialInstance = false;
        [SerializeField] private bool dontDestroyOnLoad = false;
        [SerializeField] private int sortingOrder = 32000;
        [SerializeField] private bool hideOnAwake = true;

        [Header("Events")]
        public UnityEvent OnStarted;
        public UnityEvent OnPowerOffStarted;
        public UnityEvent OnPowerOnStarted;
        public UnityEvent OnPowerOffCompleted;
        public UnityEvent OnPowerOnCompleted;
        public UnityEvent OnClosed;
        public UnityEvent OnFinished;

        private Material runtimeMaterial;
        private Coroutine routine;
        private float manualTime;
        private float holdSeed = 31.73f;

        private static readonly int ModeID = Shader.PropertyToID("_Mode");
        private static readonly int ProgressID = Shader.PropertyToID("_Progress");
        private static readonly int ManualTimeID = Shader.PropertyToID("_ManualTime");
        private static readonly int AspectID = Shader.PropertyToID("_Aspect");
        private static readonly int HoldSeedID = Shader.PropertyToID("_HoldSeed");
        private static readonly int SceneGlitchIntensityID = Shader.PropertyToID("_SceneGlitchIntensity");
        private static readonly int RGBSplitID = Shader.PropertyToID("_RGBSplit");
        private static readonly int HorizontalTearID = Shader.PropertyToID("_HorizontalTear");
        private static readonly int WaveDistortionID = Shader.PropertyToID("_WaveDistortion");
        private static readonly int FineNoiseID = Shader.PropertyToID("_FineNoise");
        private static readonly int HoldBurstIntensityID = Shader.PropertyToID("_HoldBurstIntensity");
        private static readonly int HoldLineDensityID = Shader.PropertyToID("_HoldLineDensity");
        private static readonly int HoldVerticalSpikeIntensityID = Shader.PropertyToID("_HoldVerticalSpikeIntensity");
        private static readonly int StarEdgeID = Shader.PropertyToID("_StarEdge");
        private static readonly int StarSharpnessID = Shader.PropertyToID("_StarSharpness");
        private static readonly int StarHorizontalReachID = Shader.PropertyToID("_StarHorizontalReach");
        private static readonly int StarVerticalReachID = Shader.PropertyToID("_StarVerticalReach");
        private static readonly int StarHorizontalThicknessID = Shader.PropertyToID("_StarHorizontalThickness");
        private static readonly int StarVerticalThicknessID = Shader.PropertyToID("_StarVerticalThickness");
        private static readonly int StarTipWidthID = Shader.PropertyToID("_StarTipWidth");
        private static readonly int StarIntensityID = Shader.PropertyToID("_StarIntensity");
        private static readonly int StarGlowIntensityID = Shader.PropertyToID("_StarGlowIntensity");
        private static readonly int FlashIntensityID = Shader.PropertyToID("_FlashIntensity");
        private static readonly int AfterimageIntensityID = Shader.PropertyToID("_AfterimageIntensity");
        private static readonly int AfterimageOffsetID = Shader.PropertyToID("_AfterimageOffset");

        private void Awake()
        {
            if (Instance != null && Instance != this) Debug.LogWarning($"Duplicate {nameof(TVStarTransitionController)} found.", this);
            else Instance = this;
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            PrepareMaterial();
            ApplyProfile();
            if (rootCanvas != null) rootCanvas.sortingOrder = sortingOrder;
            if (hideOnAwake) HideImmediate();
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
            if (Application.isPlaying && makeMaterialInstance && runtimeMaterial != null) Destroy(runtimeMaterial);
        }

        public void PlayPowerOffHold(bool showUIAfterHold = false, Action onFinished = null) => PlayRoutineInternal(PowerOffHoldRoutine(showUIAfterHold, onFinished));
        public void PlayPowerOnRelease(Action onFinished = null) => PlayRoutineInternal(PowerOnReleaseRoutine(onFinished));
        public void PlayDeath() => PlayPowerOffHold(false, null);
        public void PlayClear() => PlayPowerOnRelease(null);

        public void HideImmediate()
        {
            if (routine != null) { StopCoroutine(routine); routine = null; }
            PrepareMaterial();
            ApplyProfile();
            SetMode(0f, 0f);
            SetResultUI(0f, false, false);
        }

        private void PlayRoutineInternal(IEnumerator enumerator)
        {
            PrepareMaterial();
            ApplyProfile();
            if (runtimeMaterial == null)
            {
                Debug.LogError("Transition material is missing. Assign M_TVStarGlitchV12 to both the controller and the URP Full Screen Pass Renderer Feature.", this);
                return;
            }
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(enumerator);
        }

        private IEnumerator PowerOffHoldRoutine(bool showUIAfterHold, Action onFinished)
        {
            OnStarted?.Invoke();
            OnPowerOffStarted?.Invoke();

            SetResultUI(0f, false, false);
            SetNewHoldSeed();
            yield return RunPhase(1f, profile.shutdownGlitchDuration);
            yield return RunPhase(2f, profile.shutdownStarDuration);
            yield return RunPhase(3f, profile.holdIntroDuration);
            SetMode(3f, 1f);
            SetResultUI(showUIAfterHold ? 1f : 0f, showUIAfterHold, showUIAfterHold);
            OnClosed?.Invoke();
            OnPowerOffCompleted?.Invoke();
            onFinished?.Invoke();
            routine = null;
        }

        private IEnumerator PowerOnReleaseRoutine(Action onFinished)
        {
            OnStarted?.Invoke();
            OnPowerOnStarted?.Invoke();

            SetResultUI(0f, false, false);
            SetMode(3f, 1f);
            yield return null;
            yield return RunPhase(4f, profile.holdDissolveDuration);
            SetMode(5f, 1f);
            if (profile.blackHoldDuration > 0f) yield return new WaitForSecondsRealtime(profile.blackHoldDuration);
            yield return RunPhase(6f, profile.powerOnFlashDuration);
            SetMode(0f, 0f);
            SetResultUI(0f, false, false);
            OnFinished?.Invoke();
            OnPowerOnCompleted?.Invoke();
            onFinished?.Invoke();
            routine = null;
        }

        private IEnumerator RunPhase(float mode, float duration)
        {
            if (duration <= 0f) { SetMode(mode, 1f); yield break; }
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                SetMode(mode, timer / duration);
                yield return null;
            }
            SetMode(mode, 1f);
        }

        private void PrepareMaterial()
        {
            if (runtimeMaterial != null) return;
            if (transitionMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/IWantGoHome/TVStarGlitchV12");
                if (shader != null) transitionMaterial = new Material(shader) { name = "M_TVStarGlitchV12_Runtime" };
            }
            if (transitionMaterial == null) return;
            runtimeMaterial = makeMaterialInstance && Application.isPlaying ? new Material(transitionMaterial) : transitionMaterial;
        }

        private void ApplyProfile()
        {
            if (runtimeMaterial == null || profile == null) return;
            runtimeMaterial.SetFloat(SceneGlitchIntensityID, profile.sceneGlitchIntensity);
            runtimeMaterial.SetFloat(RGBSplitID, profile.rgbSplit);
            runtimeMaterial.SetFloat(HorizontalTearID, profile.horizontalTear);
            runtimeMaterial.SetFloat(WaveDistortionID, profile.waveDistortion);
            runtimeMaterial.SetFloat(FineNoiseID, profile.fineNoise);
            runtimeMaterial.SetFloat(HoldBurstIntensityID, profile.holdBurstIntensity);
            runtimeMaterial.SetFloat(HoldLineDensityID, profile.holdLineDensity);
            runtimeMaterial.SetFloat(HoldVerticalSpikeIntensityID, profile.holdVerticalSpikeIntensity);
            runtimeMaterial.SetFloat(StarEdgeID, profile.starEdgeSoftness);
            runtimeMaterial.SetFloat(StarSharpnessID, profile.starSharpness);
            runtimeMaterial.SetFloat(StarHorizontalReachID, profile.starHorizontalReach);
            runtimeMaterial.SetFloat(StarVerticalReachID, profile.starVerticalReach);
            runtimeMaterial.SetFloat(StarHorizontalThicknessID, profile.starHorizontalThickness);
            runtimeMaterial.SetFloat(StarVerticalThicknessID, profile.starVerticalThickness);
            runtimeMaterial.SetFloat(StarTipWidthID, profile.starTipWidth);
            runtimeMaterial.SetFloat(StarIntensityID, profile.starIntensity);
            runtimeMaterial.SetFloat(StarGlowIntensityID, profile.starGlowIntensity);
            runtimeMaterial.SetFloat(FlashIntensityID, profile.flashIntensity);
            runtimeMaterial.SetFloat(AfterimageIntensityID, profile.afterimageIntensity);
            runtimeMaterial.SetFloat(AfterimageOffsetID, profile.afterimageOffset);
            runtimeMaterial.SetFloat(HoldSeedID, holdSeed);
            UpdateAspect();
        }

        private void SetMode(float mode, float progress)
        {
            if (runtimeMaterial == null) return;
            runtimeMaterial.SetFloat(ModeID, mode);
            runtimeMaterial.SetFloat(ProgressID, Mathf.Clamp01(progress));
            runtimeMaterial.SetFloat(ManualTimeID, manualTime);
            runtimeMaterial.SetFloat(HoldSeedID, holdSeed);
            UpdateAspect();
        }

        private void SetNewHoldSeed()
        {
            holdSeed = UnityEngine.Random.Range(1f, 10000f);
            if (runtimeMaterial != null) runtimeMaterial.SetFloat(HoldSeedID, holdSeed);
        }

        private void SetResultUI(float alpha, bool interactable, bool blocksRaycasts)
        {
            if (resultUIGroup == null) return;
            resultUIGroup.alpha = Mathf.Clamp01(alpha);
            resultUIGroup.interactable = interactable;
            resultUIGroup.blocksRaycasts = blocksRaycasts;
        }

        private void UpdateAspect()
        {
            if (runtimeMaterial == null) return;
            float height = Mathf.Max(1, Screen.height);
            runtimeMaterial.SetFloat(AspectID, Mathf.Max(0.1f, Screen.width / height));
        }
    }
}
