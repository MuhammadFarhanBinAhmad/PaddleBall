using TMPro;
using UnityEngine;
using System.Collections;
using FMOD.Studio;
using UnityEngine.UI;

[System.Serializable]

public class ComboBG
{
    public int comboThreshold;
    public Sprite _bg;
    public Sprite _magicCircle;
}
public class BallUIManager : MonoBehaviour
{
    Ball _ballManager;
    PaddleFeedbackManager _paddleFeedbackManager;

    [Header("ComboUI")]
    [SerializeField] TextMeshProUGUI _currentComboText;
    [SerializeField] int _comboParticleThreshold;
    [SerializeField] int _wordTextSize;
    [SerializeField] ComboBG[] _comboBGs;
    [SerializeField] Image _bg, _magicCircle;
    [Header("DurabiltiyUI")]
    [SerializeField] Image _durabilityCircle;

    [Header("Animation")]
    [SerializeField] AnimationCurve easeOutElastic;
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] float _startingscaleMultiplier;
    [SerializeField] float _increasescaleMultiplier;
    [SerializeField] float _currentscaleMultiplier;
    [SerializeField] float _capscaleMultiplier;
    [Header("Magic Circle Spin")]
    [SerializeField] float _initialSpinSpeed = 720f;
    [SerializeField] float _spinSlowdownDuration = 2f;


    [Header("Combo Popup")]
    [SerializeField] RectTransform _comboPopup;
    [SerializeField] float _comboPopupOffset = 150f;
    [SerializeField] float _comboPopupDuration = 0.25f;

    Vector2 _comboShownPosition;
    Vector2 _comboHiddenPosition;

    Coroutine comboPopupAnim;
    bool _comboPopupShown;

    Coroutine magicCircleSpin;
    bool _audioPlayed;
    EventInstance _paddleHitCombo;

    Coroutine comboAnim;

    private void Awake()
    {
        _ballManager = FindAnyObjectByType<Ball>();
        _paddleFeedbackManager = FindAnyObjectByType < PaddleFeedbackManager>();

    }
    void Start()
    {

        _paddleHitCombo = AudioManager.Instance.CreateEventInstance(
                FmodEvent.Instance.sfx_onPaddleComboHit
            );

        _ballManager.OnBrickHit += UpdateComboUI;
        _ballManager.OnBrickHit += PlayComboAudio;
        _ballManager.OnBrickHit += UpdateDurabilityUI;
        _ballManager.OnBallReset += UpdateComboUI;
        _ballManager.OnBallReset += UpdateDurabilityUI;
        _paddleFeedbackManager.OnHitBall += UpdateDurabilityUI;

        if (_comboPopup == null)
            _comboPopup = GetComponent<RectTransform>();

        _comboShownPosition = _comboPopup.anchoredPosition;

        _comboHiddenPosition =
            _comboShownPosition + Vector2.down * _comboPopupOffset;

        // Start hidden
        _comboPopup.anchoredPosition = _comboHiddenPosition;

        UpdateComboUI();
    }

    private void OnDisable()
    {
        _ballManager.OnBrickHit -= UpdateComboUI;
        _ballManager.OnBrickHit -= PlayComboAudio;
        _ballManager.OnBrickHit -= UpdateDurabilityUI;
        _ballManager.OnBallReset -= UpdateComboUI;
        _ballManager.OnBallReset -= UpdateDurabilityUI;
        _paddleFeedbackManager.OnHitBall -= UpdateDurabilityUI;
    }

    public void UpdateComboUI()
    {
        int combo = _ballManager._currentCombo;

        // First combo hit
        if (combo == 1 && !_comboPopupShown)
        {
            _comboPopupShown = true;
            PlayComboPopup(true);
        }
        // Combo lost / reset
        else if (combo <= 0 && _comboPopupShown)
        {
            _comboPopupShown = false;
            PlayComboPopup(false);
        }

        UpdateComboPerformanceNumber();
        UpdateComboPerformanceText();

        if (comboAnim != null)
            StopCoroutine(comboAnim);

        comboAnim = StartCoroutine(AnimateCombo());

        if (magicCircleSpin != null)
            StopCoroutine(magicCircleSpin);

        magicCircleSpin = StartCoroutine(SpinMagicCircle());
    }
    void UpdateComboPerformanceNumber()
    {
        if (_ballManager._currentCombo > 0)
        {
            _currentComboText.text = _ballManager._currentCombo.ToString();
            if (_ballManager._currentCombo % _comboParticleThreshold == 0 && _currentscaleMultiplier < _capscaleMultiplier)
            {
                _currentscaleMultiplier += _increasescaleMultiplier;
            }
        }
        else
        {
            _currentscaleMultiplier = _startingscaleMultiplier;
            _currentComboText.text = "";
            return;
        }
    }

    void UpdateComboPerformanceText()
    {
        int combo = _ballManager._currentCombo;

        for (int i = _comboBGs.Length - 1; i >= 0; i--)
        {
            if (combo >= _comboBGs[i].comboThreshold)
            {
                _bg.sprite = _comboBGs[i]._bg;
                _magicCircle.sprite = _comboBGs[i]._magicCircle;
                return;
            }
        }
    }
    void PlayComboPopup(bool show)
    {
        if (comboPopupAnim != null)
            StopCoroutine(comboPopupAnim);

        Vector2 startPosition = _comboPopup.anchoredPosition;
        Vector2 targetPosition = show
            ? _comboShownPosition
            : _comboHiddenPosition;

        comboPopupAnim = StartCoroutine(
            AnimateComboPopup(startPosition, targetPosition)
        );
    }
    void PlayComboAudio()
    {
        //int combo = _ballManager._currentCombo;

        //for (int i = 1; i < _comboPerformances.Length-1; i++)
        //{
        //    if (combo == _comboPerformances[i].comboThreshold)
        //    {
        //        // Play audio for this performance tier
        //        _paddleHitCombo.setParameterByName("ComboPitch", i);
        //        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleComboHit, transform.position);
        //        return;
        //    }
        //}
    }
    IEnumerator AnimateCombo()
    {
        Transform n = _currentComboText.transform;
        Transform t = _magicCircle.transform;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * _currentscaleMultiplier;

        float time = 0f;

        while (time < animationDuration)
        {
            float normalized = time / animationDuration;
            float curveValue = easeOutElastic.Evaluate(normalized);

            n.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);
            t.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        n.localScale = Vector3.one;
        t.localScale = Vector3.one;
    }
    IEnumerator SpinMagicCircle()
    {
        float time = 0f;

        while (time < _spinSlowdownDuration)
        {
            float normalized = time / _spinSlowdownDuration;

            // Starts at 1 and gradually reaches 0
            float speedMultiplier = 1f - normalized;

            float rotationAmount = _initialSpinSpeed * speedMultiplier * Time.deltaTime;

            _magicCircle.transform.Rotate(0f, 0f, rotationAmount);

            time += Time.deltaTime;

            yield return null;
        }
    }
    IEnumerator AnimateComboPopup(
    Vector2 startPosition,
    Vector2 targetPosition)
    {
        float time = 0f;

        while (time < _comboPopupDuration)
        {
            float normalized = time / _comboPopupDuration;

            _comboPopup.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    normalized
                );

            time += Time.deltaTime;
            yield return null;
        }

        _comboPopup.anchoredPosition = targetPosition;
        comboPopupAnim = null;
    }
    public void UpdateDurabilityUI()
    {
        _durabilityCircle.fillAmount = _ballManager.GetDurabilityPercentage();
    }
}
