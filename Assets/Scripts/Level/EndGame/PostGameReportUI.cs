
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum POST_GAME_STAT
{
    // Tower Stats
    BRICK_TOTAL,
    TOTAL_ESSENCE_EARN,
    TOTAL_PURE_ESSENCE_EARN,
    TOTAL_PURE_ESSENCE_SPENT,
    TOTAL_DAMAGE_TAKEN,
    HIGHEST_DAMAGE_TAKEN,

    // Ball Stats
    MAX_DAMAGE,
    TOTAL_DAMAGE,
    AVR_DAMAGE,
    TOTAL_CARD,
    TOTAL_COMMON_CARD,
    TOTAL_UNCOMMON_CARD,
    TOTAL_RARE_CARD,
    TOTAL_LEGENDARY_CARD,
    TOTAL_SPELL,
    TOTAL_EXPLOSION_SPELL,
    TOTAL_DISCHARGE_SPELL,
    TOTAL_TOXIC_SPELL,
    TOTAL_CRIT_SPELL,
    TOTAL_TIME_REDIRECT_BALL,
    TOTAL_BALL_DESTROYED,
    HIGHEST_COMBO,

    // Enemy Stats
    TOTAL_BRICK_DESTROYED
}

[Serializable]
public class PostGameStatEntry
{
    public POST_GAME_STAT _statType;
    [Header("Display")]
    public string _statName;
    [Header("UI References")]
    public GameObject _statUI;
    public TMP_Text _valueText;
}

public class PostGameReportUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager _statManager;

    [Header("Stats to Display")]
    [SerializeField]
    private List<PostGameStatEntry> _statEntries = new();

    [Header("Animation Settings")]
    [SerializeField] private float _countDuration = 0.5f;
    [SerializeField] private float _delayBetweenStats = 0.15f;
    [SerializeField]
    private AnimationCurve _countCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _reportCoroutine;
    private bool _isPlaying;
    private bool _isCompleted;

    public bool IsPlaying => _isPlaying;
    public bool IsCompleted => _isCompleted;

    public event Action OnReportCompleted;

    private void Awake()
    {
        if (_statManager == null)
            _statManager = FindAnyObjectByType<StatManager>();

        HideAllStats();
    }

    private void OnDisable()
    {
        _reportCoroutine = null;
        _isPlaying = false;
    }

    #region Report Control

    public void StartReport()
    {
        if (_isPlaying)
            return;

        if (_statManager == null)
        {
            Debug.LogError("StatManager not found!");
            return;
        }

        if (_reportCoroutine != null)
            StopCoroutine(_reportCoroutine);

        _isCompleted = false;
        _isPlaying = true;

        HideAllStats();

        _reportCoroutine = StartCoroutine(ShowStatsRoutine());
    }

    private IEnumerator ShowStatsRoutine()
    {
        foreach (PostGameStatEntry entry in _statEntries)
        {
            if (entry._statUI == null || entry._valueText == null)
                continue;

            int targetValue = GetStatValue(entry._statType);

            entry._statUI.SetActive(true);

            entry._valueText.text = FormatStatText(entry._statName, 0);

            yield return StartCoroutine(
                AnimateStatValue(
                    entry._valueText,
                    entry._statName,
                    targetValue
                )
            );

            yield return new WaitForSecondsRealtime(
                _delayBetweenStats
            );
        }

        CompleteReport();
    }

    private IEnumerator AnimateStatValue(
    TMP_Text valueText,
    string statName,
    int targetValue)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0f, _countDuration);

        if (duration <= 0f)
        {
            valueText.text = FormatStatText(statName, targetValue);
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = _countCurve.Evaluate(t);

            int currentValue = Mathf.RoundToInt(
                Mathf.LerpUnclamped(0, targetValue, curveValue)
            );

            valueText.text = FormatStatText(statName, currentValue);

            yield return null;
        }

        valueText.text = FormatStatText(statName, targetValue);
    }

    public void SkipReportAnimation()
    {
        if (!_isPlaying)
            return;

        if (_reportCoroutine != null)
        {
            StopCoroutine(_reportCoroutine);
            _reportCoroutine = null;
        }

        ShowAllStatsInstantly();

        CompleteReport();
    }

    private void CompleteReport()
    {
        _isPlaying = false;
        _isCompleted = true;
        _reportCoroutine = null;

        OnReportCompleted?.Invoke();
    }

    #endregion

    #region UI Management

    private void HideAllStats()
    {
        foreach (PostGameStatEntry entry in _statEntries)
        {
            if (entry._statUI != null)
                entry._statUI.SetActive(false);

            if (entry._valueText != null)
                entry._valueText.text = FormatStatText(entry._statName, 0);
        }
    }

    private void ShowAllStatsInstantly()
    {
        foreach (PostGameStatEntry entry in _statEntries)
        {
            if (entry._statUI == null || entry._valueText == null)
                continue;

            int value = GetStatValue(entry._statType);

            entry._valueText.text = FormatStatText(entry._statName, value);
            entry._statUI.SetActive(true);
        }
    }
    private string FormatStatText(string statName, int value)
    {
        return $"{statName} : {value:N0}";
    }
    #endregion

    #region Stat Retrieval

    private int GetStatValue(POST_GAME_STAT stat)
    {
        switch (stat)
        {
            // Tower Stats
            case POST_GAME_STAT.BRICK_TOTAL:
                return _statManager._brickTotal;

            case POST_GAME_STAT.TOTAL_ESSENCE_EARN:
                return _statManager._totalEssenceEarn;

            case POST_GAME_STAT.TOTAL_PURE_ESSENCE_EARN:
                return _statManager._totalPureEssenceEarn;

            case POST_GAME_STAT.TOTAL_PURE_ESSENCE_SPENT:
                return _statManager._totalPureEssenceSpent;

            case POST_GAME_STAT.TOTAL_DAMAGE_TAKEN:
                return _statManager._totalDamageTaken;

            case POST_GAME_STAT.HIGHEST_DAMAGE_TAKEN:
                return _statManager._highestDamageTaken;

            // Ball Stats
            case POST_GAME_STAT.MAX_DAMAGE:
                return _statManager._maxDamage;

            case POST_GAME_STAT.TOTAL_DAMAGE:
                return _statManager._totalDamage;

            case POST_GAME_STAT.AVR_DAMAGE:
                return _statManager._avrDamage;

            case POST_GAME_STAT.TOTAL_CARD:
                return _statManager._totalCard;

            case POST_GAME_STAT.TOTAL_COMMON_CARD:
                return _statManager._totalCommonCard;

            case POST_GAME_STAT.TOTAL_UNCOMMON_CARD:
                return _statManager._totalUncommonCard;

            case POST_GAME_STAT.TOTAL_RARE_CARD:
                return _statManager._totalRareCard;

            case POST_GAME_STAT.TOTAL_LEGENDARY_CARD:
                return _statManager._totalLegendaryCard;

            case POST_GAME_STAT.TOTAL_SPELL:
                return _statManager._totalSpell;

            case POST_GAME_STAT.TOTAL_EXPLOSION_SPELL:
                return _statManager._totalExplosionSpell;

            case POST_GAME_STAT.TOTAL_DISCHARGE_SPELL:
                return _statManager._totalDischargeSpell;

            case POST_GAME_STAT.TOTAL_TOXIC_SPELL:
                return _statManager._totalToxicSpell;

            case POST_GAME_STAT.TOTAL_CRIT_SPELL:
                return _statManager._totalCritSpell;

            case POST_GAME_STAT.TOTAL_TIME_REDIRECT_BALL:
                return _statManager._totalTimeRedirectBall;

            case POST_GAME_STAT.TOTAL_BALL_DESTROYED:
                return _statManager._totalBallDestroyed;

            case POST_GAME_STAT.HIGHEST_COMBO:
                return _statManager._highestCombo;

            // Enemy Stats
            case POST_GAME_STAT.TOTAL_BRICK_DESTROYED:
                return _statManager._totalBrickDestroyed;

            default:
                return 0;
        }
    }

    #endregion
}
