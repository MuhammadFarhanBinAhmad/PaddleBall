
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutScreenEventCutScene : BaseCutsceneEvent
{
    [SerializeField] Image BlackImage;
    [SerializeField] TextMeshProUGUI _gameOverText;
    [SerializeField] AnimationCurve _fadeInEffectLerp;
    [SerializeField] float _animationFadeInDuration;
    [SerializeField] GameObject _buttonParents;
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
        TimeManager.StopTime();

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (BlackImage == null || _gameOverText == null)
        {
            Debug.LogWarning("BlackImage or GameOverText is null.");
            EndEvent();
            yield break;
        }

        BlackImage.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);

        // Get their individual colours.
        Color imageColor = BlackImage.color;
        Color textColor = _gameOverText.color;

        // Set starting alpha without changing RGB.
        imageColor.a = _startAlpha;
        textColor.a = _startAlpha;

        BlackImage.color = imageColor;
        _gameOverText.color = textColor;

        float time = 0f;

        while (time < _animationFadeInDuration)
        {
            float t = time / _animationFadeInDuration;

            float curveT = _fadeInEffectLerp != null
                ? _fadeInEffectLerp.Evaluate(t)
                : t;

            float currentAlpha = Mathf.LerpUnclamped(
                _startAlpha,
                _endAlpha,
                curveT
            );

            // Only modify alpha.
            imageColor.a = currentAlpha;
            textColor.a = currentAlpha;

            BlackImage.color = imageColor;
            _gameOverText.color = textColor;

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure both reach the exact final alpha.
        imageColor.a = _endAlpha;
        textColor.a = _endAlpha;

        BlackImage.color = imageColor;
        _gameOverText.color = textColor;

        yield return new WaitForSecondsRealtime(_animationFadeInDuration);

        _buttonParents.SetActive(true);

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
