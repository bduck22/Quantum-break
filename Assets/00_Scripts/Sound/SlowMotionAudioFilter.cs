using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SlowMotionAudioFilter : MonoBehaviour
{
    private enum FilterTransitionType
    {
        None,
        Enter,
        Exit
    }

    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string cutoffParameter = "MasterLowpassCutoff";
    [SerializeField] private string resonanceParameter = "MasterLowpassResonance";

    [Header("Normal")]
    [SerializeField] private float normalCutoff = 22000f;
    [SerializeField] private float normalResonance = 1f;

    [Header("Slow Motion")]
    [SerializeField] private float slowCutoff = 1200f;
    [SerializeField] private float slowResonance = 1.6f;

    [Header("Transition")]
    [SerializeField] private float enterDuration = 0.08f;
    [SerializeField] private float exitDuration = 0.18f;

    private Coroutine filterRoutine;
    private FilterTransitionType currentTransitionType = FilterTransitionType.None;

    public void EnterSlowMotion()
    {
        // Enter가 이미 실행 중이면 중복 실행 방지
        if (filterRoutine != null && currentTransitionType == FilterTransitionType.Enter)
        {
            return;
        }

        // Exit 중이었다면 끊고 Enter로 전환
        StopCurrentFilterRoutine();

        filterRoutine = StartCoroutine(FilterTransition(
            slowCutoff,
            slowResonance,
            enterDuration,
            FilterTransitionType.Enter
        ));
    }

    public void ExitSlowMotion()
    {
        // Exit는 무조건 바로 실행
        // Enter 중이든 Exit 중이든 기존 코루틴을 끊고 새 Exit 실행
        StopCurrentFilterRoutine();

        filterRoutine = StartCoroutine(FilterTransition(
            normalCutoff,
            normalResonance,
            exitDuration,
            FilterTransitionType.Exit
        ));
    }

    private void StopCurrentFilterRoutine()
    {
        if (filterRoutine == null)
        {
            return;
        }

        StopCoroutine(filterRoutine);
        filterRoutine = null;
        currentTransitionType = FilterTransitionType.None;
    }

    private IEnumerator FilterTransition(
        float targetCutoff,
        float targetResonance,
        float duration,
        FilterTransitionType transitionType
    )
    {
        currentTransitionType = transitionType;

        masterMixer.GetFloat(cutoffParameter, out float startCutoff);
        masterMixer.GetFloat(resonanceParameter, out float startResonance);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;
            t = Mathf.Clamp01(t);

            // SmoothStep
            t = t * t * (3f - 2f * t);

            float cutoff = Mathf.Lerp(startCutoff, targetCutoff, t);
            float resonance = Mathf.Lerp(startResonance, targetResonance, t);

            masterMixer.SetFloat(cutoffParameter, cutoff);
            masterMixer.SetFloat(resonanceParameter, resonance);

            yield return null;
        }

        masterMixer.SetFloat(cutoffParameter, targetCutoff);
        masterMixer.SetFloat(resonanceParameter, targetResonance);

        filterRoutine = null;
        currentTransitionType = FilterTransitionType.None;
    }
}