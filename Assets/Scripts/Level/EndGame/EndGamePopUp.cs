
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePopup : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private RectTransform _popupRect;

    [Header("Animation Effects")]
    [SerializeField] private SOLerpAnimationEffect _onPopInEffect;
    [SerializeField] private SOLerpAnimationEffect _onPopOutEffect;

    private Action _onPopupCompleted;
    private Coroutine _animationCoroutine;

    private bool _isClosing;
    private Vector3 _originalScale;
    [SerializeField] PostGameReportUI _postGameReportUI;
    [SerializeField] PostGameTowerUI _postGameTowerUI;
    private void Awake()
    {
        _originalScale = _popupRect.localScale;
        _continueButton.onClick.AddListener(ClosePopup);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(_postGameReportUI != null)
                _postGameReportUI.SkipReportAnimation();
            if (_postGameTowerUI != null)
                _postGameTowerUI.SkipTowerAnimation();
        }
    }
    public void ShowPopup(Action onComplete)
    {
        _onPopupCompleted = onComplete;
        _isClosing = false;

        gameObject.SetActive(true);

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        _continueButton.interactable = false;

        yield return StartCoroutine(
            AnimateScale(
                Vector3.zero,
                _originalScale,
                _onPopInEffect
            )
        );

        if(_postGameReportUI != null)
            _postGameReportUI?.StartReport();

        if (_postGameTowerUI != null)
            _postGameTowerUI?.StartTowerAnimation();

        _continueButton.interactable = true;
        _animationCoroutine = null;
    }

    public void ClosePopup()
    {
        if (_postGameReportUI != null &&
            _postGameReportUI.IsPlaying)
        {
            _postGameReportUI.SkipReportAnimation();
            return;
        }
        if (_postGameTowerUI != null &&
            _postGameTowerUI.IsBuilding)
        {
            _postGameTowerUI.SkipTowerAnimation();
            return;
        }

        if (_isClosing || !gameObject.activeInHierarchy)
            return;

        _isClosing = true;

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(PopOut());
    }

    private IEnumerator PopOut()
    {
        _continueButton.interactable = false;

        yield return StartCoroutine(
            AnimateScale(
                _popupRect.localScale,
                Vector3.zero,
                _onPopOutEffect
            )
        );

        _animationCoroutine = null;

        Action callback = _onPopupCompleted;
        _onPopupCompleted = null;

        gameObject.SetActive(false);

        callback?.Invoke();
    }

    private IEnumerator AnimateScale(
        Vector3 startScale,
        Vector3 endScale,
        SOLerpAnimationEffect effect)
    {
        if (effect == null)
        {
            _popupRect.localScale = endScale;
            yield break;
        }

        float duration = Mathf.Max(0f, effect.animationDuration);
        float elapsed = 0f;

        _popupRect.localScale = startScale;

        if (duration <= 0f)
        {
            _popupRect.localScale = endScale;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Use ScriptableObject animation curve
            float curveValue = effect.easeOutElastic != null
                ? effect.easeOutElastic.Evaluate(t)
                : t;

            // Control overshoot using the multiplier
            float multiplier = effect._capscaleMultiplier;

            // Apply multiplier only to the overshoot portion
            if (curveValue > 1f)
            {
                curveValue = 1f + (curveValue - 1f) * multiplier;
            }

            _popupRect.localScale = Vector3.LerpUnclamped(
                startScale,
                endScale,
                curveValue
            );

            yield return null;
        }

        _popupRect.localScale = endScale;
    }

    private void OnDestroy()
    {
        if (_continueButton != null)
            _continueButton.onClick.RemoveListener(ClosePopup);
    }


}
