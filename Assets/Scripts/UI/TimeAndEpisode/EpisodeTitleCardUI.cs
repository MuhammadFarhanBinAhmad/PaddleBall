using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EpisodeTitleCardUI : MonoBehaviour
{
    BrickGenerator _brickGenerator;

    [SerializeField] GameObject _titleCardGameObject;
    [SerializeField] Image _titleCardImage;
    [Header("Curve")]
    [SerializeField] private AnimationCurve _fadeInEffectLerp;
    [SerializeField] private AnimationCurve _popOutEffectLerp;
    [Header("EffectsValue")]
    [SerializeField] float _startAlpha = 0f;
    [SerializeField] float _endAlpha = 1f;
    [SerializeField] private float _cardStartScaleMultiplier;
    [SerializeField] private float _cardEndScaleMultiplier;
    [SerializeField] private float _displayDuration;
    [SerializeField] private float _animationFadeInDuration;
    [SerializeField] private float _animationPopOutDuration;

    Vector3 _cardOriginalScale;
    Coroutine _titleCardRoutine;

    private void Awake()
    {
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();


    }
    private void Start()
    {
        _cardOriginalScale = _titleCardGameObject.transform.localScale;
        _titleCardGameObject.SetActive(false);

        if (_titleCardImage != null)
        {
            Color c = _titleCardImage.color;
            c.a = _startAlpha;
            _titleCardImage.color = c;
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            PlayTitleCardAnim();
    }
    public void PlayTitleCardAnim()
    {
        if (_titleCardRoutine != null)
            StopCoroutine(_titleCardRoutine);

        TimeManager.StopTime();
        _titleCardRoutine = StartCoroutine(TitleCardSequence());
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

        _brickGenerator.StartFirstWaveOfEpisode();
    }

    void SetAlpha(float a)
    {
        if (_titleCardImage == null) return;

        Color c = _titleCardImage.color;
        c.a = a;
        _titleCardImage.color = c;
    }
}
