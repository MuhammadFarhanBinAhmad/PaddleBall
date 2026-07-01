using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EpisodeTitleCardUI : MonoBehaviour
{
    SO_BossIntroText _BossIntroText;

    [SerializeField] GameObject _titleCardGameObject;
    [SerializeField] GameObject _defeatTitleCardGameObject;

    [SerializeField] Image _titleCardBGImage;
    [Header("Curve")]
    [SerializeField] AnimationCurve _fadeInEffectLerp;
    [SerializeField] AnimationCurve _popOutEffectLerp;
    [Header("EffectsValue")]
    [SerializeField] float _startAlpha = 0f;
    [SerializeField] float _endAlpha = 1f;
    [SerializeField] float _cardStartScaleMultiplier;
    [SerializeField] float _cardEndScaleMultiplier;
    [SerializeField] float _displayDuration;
    [SerializeField] float _animationFadeInDuration;
    [SerializeField] float _animationPopOutDuration;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI _bossName;
    [SerializeField] TextMeshProUGUI _bossTagLine;
    [SerializeField] TextMeshProUGUI _bossDefeateTagline;

    Vector3 _cardOriginalScale;
    Coroutine _titleCardRoutine;

    private void Start()
    {
        _cardOriginalScale = _titleCardGameObject.transform.localScale;
        _titleCardGameObject.SetActive(false);

        if (_titleCardBGImage != null)
        {
            Color c = _titleCardBGImage.color;
            c.a = _startAlpha;
            _titleCardBGImage.color = c;
        }
    }

    public void PlayTitleCardAnim()
    {

        if (_titleCardRoutine != null)
            StopCoroutine(_titleCardRoutine);

        TimeManager.StopTime();
        SetTitleCard();
        _titleCardRoutine = StartCoroutine(TitleCardSequence());
    }
    public void PlayBossDefeatTitleCardAnim()
    {
        if (_titleCardRoutine != null)
            StopCoroutine(_titleCardRoutine);

        TimeManager.StopTime();
        SetTitleCard();
        _titleCardRoutine = StartCoroutine(DeafeatBossTitleCardSequence());
    }
    void SetTitleCard()
    {
        _bossName.text = _BossIntroText._name;
        _bossTagLine.text = _BossIntroText._tagline;
        _bossDefeateTagline.text = _BossIntroText._defeatTagline;
    }

    IEnumerator TitleCardSequence()
    {
        // -------------------------
        // FADE IN (while inactive)
        // -------------------------
        float time = 0f;

        while (time < _animationFadeInDuration)
        {
            float t = time / _animationFadeInDuration;
            float curveT = _fadeInEffectLerp != null ? _fadeInEffectLerp.Evaluate(t) : t;

            SetAlpha(Mathf.Lerp(_startAlpha, _endAlpha, curveT));

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(_endAlpha);

        // NOW ACTIVATE
        _titleCardGameObject.SetActive(true);

        // -------------------------
        // POP IN SCALE
        // -------------------------
        _titleCardGameObject.transform.localScale = _cardOriginalScale;

        // -------------------------
        // STAY
        // -------------------------
        yield return new WaitForSecondsRealtime(_displayDuration);

        // -------------------------
        // POP OUT SCALE
        // -------------------------
        time = 0f;
        while (time < _animationPopOutDuration)
        {
            float t = time / _animationPopOutDuration;
            float curveT = _popOutEffectLerp != null ? _popOutEffectLerp.Evaluate(t) : t;

            _titleCardGameObject.transform.localScale = Vector3.Lerp(
                _cardOriginalScale,
                _cardOriginalScale * _cardEndScaleMultiplier,
                curveT
            );

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _titleCardGameObject.transform.localScale = _cardOriginalScale * _cardEndScaleMultiplier;

        // -------------------------
        // FINAL RESET + HIDE
        // -------------------------
        SetAlpha(_startAlpha);
        _titleCardGameObject.SetActive(false);
        _titleCardRoutine = null;
        TimeManager.ResetTimeScale();
    }
    IEnumerator DeafeatBossTitleCardSequence()
    {
        // -------------------------
        // FADE IN (while inactive)
        // -------------------------
        float time = 0f;

        while (time < _animationFadeInDuration)
        {
            float t = time / _animationFadeInDuration;
            float curveT = _fadeInEffectLerp != null ? _fadeInEffectLerp.Evaluate(t) : t;

            SetAlpha(Mathf.Lerp(_startAlpha, _endAlpha, curveT));

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(_endAlpha);

        // NOW ACTIVATE
        _titleCardGameObject.SetActive(true);

        // -------------------------
        // POP IN SCALE
        // -------------------------
        _titleCardGameObject.transform.localScale = _cardOriginalScale;

        // -------------------------
        // STAY
        // -------------------------
        yield return new WaitForSecondsRealtime(_displayDuration);

        _defeatTitleCardGameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(_displayDuration);


        // -------------------------
        // POP OUT SCALE
        // -------------------------
        time = 0f;
        while (time < _animationPopOutDuration)
        {
            float t = time / _animationPopOutDuration;
            float curveT = _popOutEffectLerp != null ? _popOutEffectLerp.Evaluate(t) : t;

            _titleCardGameObject.transform.localScale = Vector3.Lerp(
                _cardOriginalScale,
                _cardOriginalScale * _cardEndScaleMultiplier,
                curveT
            );

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        _titleCardGameObject.transform.localScale = _cardOriginalScale * _cardEndScaleMultiplier;

        // -------------------------
        // FINAL RESET + HIDE
        // -------------------------
        SetAlpha(_startAlpha);
        _titleCardGameObject.SetActive(false);
        _defeatTitleCardGameObject.SetActive(false);
        _titleCardRoutine = null;
        TimeManager.ResetTimeScale();
    }
    void SetAlpha(float a)
    {
        if (_titleCardBGImage == null) return;

        Color c = _titleCardBGImage.color;
        c.a = a;
        _titleCardBGImage.color = c;
    }
    public void SetBossIntroText(SO_BossIntroText bit) => _BossIntroText = bit;
}
