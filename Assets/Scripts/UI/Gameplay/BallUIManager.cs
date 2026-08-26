using TMPro;
using UnityEngine;
using System.Collections;
using FMOD.Studio;

[System.Serializable]
public struct ComboPerformance
{
    public int comboThreshold;
    public string word;
    public string rankLetter;
}

public class BallUIManager : MonoBehaviour
{
    Ball _ballManager;

    [Header("ComboUI")]
    [SerializeField] TextMeshProUGUI _currentComboText;
    [SerializeField] int _comboParticleThreshold;
    [SerializeField] TextMeshProUGUI _comboPerformanceText;
    [SerializeField] ComboPerformance[] _comboPerformances;
    [SerializeField] int _rankLetterSize, _wordTextSize;

    [Header("Animation")]
    [SerializeField] AnimationCurve easeOutElastic;
    [SerializeField] float animationDuration = 0.5f;
    [SerializeField] float _startingscaleMultiplier;
    [SerializeField] float _increasescaleMultiplier;
    [SerializeField] float _currentscaleMultiplier;
    [SerializeField] float _capscaleMultiplier;

    bool _audioPlayed;
    EventInstance _paddleHitCombo;

    Coroutine comboAnim;

    private void Awake()
    {
        _ballManager = FindAnyObjectByType<Ball>();

    }
    void Start()
    {

        _paddleHitCombo = AudioManager.Instance.CreateEventInstance(FmodEvent.Instance.sfx_onPaddleComboHit);


        _ballManager.OnBrickHit += UpdateComboUI;
        _ballManager.OnBrickHit += PlayComboAudio;

        _ballManager.OnBallReset+= UpdateComboUI;
    }

    private void OnDisable()
    {
        _ballManager.OnBrickHit -= UpdateComboUI;
        _ballManager.OnBallHit -= PlayComboAudio;

        _ballManager.OnBallReset -= UpdateComboUI;
    }

    public void UpdateComboUI()
    {
        UpdateComboPerformanceNumber();
        UpdateComboPerformanceText();

        if (comboAnim != null)
            StopCoroutine(comboAnim);

        comboAnim = StartCoroutine(AnimateCombo());
    }
    void UpdateComboPerformanceNumber()
    {
        if (_ballManager._currentCombo > 0)
        {
            _currentComboText.text = _ballManager._currentCombo.ToString() + 'x';
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

        for (int i = _comboPerformances.Length - 1; i >= 0; i--)
        {
            if (combo >= _comboPerformances[i].comboThreshold)
            {
                _comboPerformanceText.text =
                    $"<size={_rankLetterSize}><b>{_comboPerformances[i].rankLetter}</b></size>" +
                    $"<size={_wordTextSize}> {_comboPerformances[i].word}</size>";
                return;
            }
        }
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
        Transform t = _comboPerformanceText.transform;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * _currentscaleMultiplier;

        float time = 0f;

        while (time < animationDuration)
        {
            float normalized = time / animationDuration;
            float curveValue = easeOutElastic.Evaluate(normalized);

            t.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);
            n.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        t.localScale = Vector3.one;
        n.localScale = Vector3.one;
    }
}
