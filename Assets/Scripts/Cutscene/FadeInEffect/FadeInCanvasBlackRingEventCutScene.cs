using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInCanvasBlackRingEventCutScene : BaseCutsceneEvent
{
    [SerializeField] Image CanvasRingImage;
    [SerializeField] AnimationCurve _fadeInEffectLerp;
    [SerializeField] float _animationFadeInDuration;
    float _startAlpha = 0f;
    float _endAlpha = 1f;
    private Coroutine _routine;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
        _fadeInEffectLerp = content._ringAnim;
        _animationFadeInDuration = content._animationFadeInDuration;
        _startAlpha = content._startAlpha;
        _endAlpha = content._endAlpha;
    }

    public override void ExecuteEvent()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (CanvasRingImage == null)
        {
            Debug.LogWarning("CanvasRingImage is null.");
            yield break;
        }

        CanvasRingImage.gameObject.SetActive(true);

        Color color = CanvasRingImage.color;
        color.a = _startAlpha;
        CanvasRingImage.color = color;

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

            CanvasRingImage.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = _endAlpha;
        CanvasRingImage.color = color;

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
