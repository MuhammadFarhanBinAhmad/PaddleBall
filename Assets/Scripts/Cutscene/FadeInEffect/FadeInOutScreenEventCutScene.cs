using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutScreenEventCutScene : BaseCutsceneEvent
{
    [SerializeField] Image BlackImage;
    [SerializeField] AnimationCurve _fadeInEffectLerp;
    [SerializeField] float _animationFadeInDuration;
    float _startAlpha = 0f;
    float _endAlpha = 1f;
    private Coroutine _routine;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
        _fadeInEffectLerp = content._FadeAnim;
        _animationFadeInDuration = content._Fadetime;
        _startAlpha = content._startFadeAlpha;
        _endAlpha = content._endFadeAlpha;
    }

    public override void ExecuteEvent()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (BlackImage == null)
        {
            Debug.LogWarning("CanvasRingImage is null.");
            yield break;
        }

        BlackImage.gameObject.SetActive(true);

        Color color = BlackImage.color;
        color.a = _startAlpha;
        BlackImage.color = color;

        float time = 0f;

        while (time < _animationFadeInDuration)
        {
            float t = time / _animationFadeInDuration;

            float curveT = _fadeInEffectLerp != null
                ? _fadeInEffectLerp.Evaluate(t)
                : t;

            color.a = Mathf.LerpUnclamped(
                _startAlpha,
                _endAlpha,
                curveT);

            BlackImage.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = _endAlpha;
        BlackImage.color = color;

        EndEvent();
    }

    public override void EndEvent()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        NotifyFinished();
    }
}
