using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    BrickPool _brickpool;
    StoreAbilityManager _storeAbilityManager;
    StoreOverlayUI _overlayUI;
    BossManager _bossManager;
    TowerManager _towerManager;
    TutorialManager _tutorialManager;

    [Header("TimeKeeper")]
    [SerializeField] int _maxGameDuration;
    int _totalDayPass;
    [SerializeField] float _fullDayDuration;
    [SerializeField] float _bossDayDuration;
    float _daysDuration;
    [SerializeField] float _currentDayDuration;
    [SerializeField] int _bossDayInterval;
    bool _firstDay = true;
    [Header("EndDayContent")]
    [SerializeField] Button _startDayButton;
    [SerializeField] GameObject _cardShopButton;

    bool _hasDayEnd;
    bool _isBossDay;
    bool _bossFightEnded;

    [Header("Timelapse")]
    [SerializeField] float _postBossSpeedMultiplier = 6f;
    float _daySpeedMultiplier = 1f;

    public Action _onStartBossDay;
    public Action _onEndBossDay;
    public Action _onBossDefeated;

    public Action _dayEnd;
    public Action _dayEndAllBrickDestroyed;
    public Action _dayStart;
    public Action _endGame;

    [Header("Game Clock")]
    //[SerializeField] float _currentGameTime;

    [Header("RealTime")]
    [SerializeField] float _currentRealTimePass;

    [Header("BackGround")]
    [SerializeField] List<Sprite> _dayBGList = new List<Sprite>();
    [SerializeField] List<Sprite> _nightBGList = new List<Sprite>();
    [SerializeField] float _backgroundFadeDuration = 1f;
    int _currentBGIndex = 1;
    [SerializeField] SpriteRenderer[] _dayBG;
    [SerializeField] SpriteRenderer[] _nightBG;
    [SerializeField] SpriteRenderer _frontCloud, _backCloud;
    [SerializeField] Color _dayFrontCloudShade, _nightFrontCloudShade;
    [SerializeField] Color _dayBackCloudShade, _nightBackCloudShade;
    [Header("Background Movement")]
    [SerializeField] float _backgroundMoveDuration;
    [SerializeField] float _backgroundMoveValue;

    // Only need the bottom boundary.
    [SerializeField] Transform _bgEndPos;

    Coroutine _backgroundMoveCoroutine;

    // Track the next sprite to display.
    int _nextDayBGIndex;
    int _nextNightBGIndex;



    bool _startDayTimer;
    bool _wasDayTime;

    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _bossManager = FindAnyObjectByType<BossManager>();
        _brickpool = FindAnyObjectByType<BrickPool>();
        _overlayUI = FindAnyObjectByType<StoreOverlayUI>();
        _towerManager = FindAnyObjectByType<TowerManager>();
        _tutorialManager = FindAnyObjectByType<TutorialManager>();
    }

    private void Start()
    {
        _dayEnd += PlayDayPassAudio;
        _dayEnd += _storeAbilityManager.SetItemAvailableToPurchase;
        _dayEnd += _storeAbilityManager.ResetRoroll;
        _dayEnd += _overlayUI.ResetItems;
        _dayEnd += _overlayUI.UpdateRerollText;
        _dayEnd += StopDayTimer;

        _dayEndAllBrickDestroyed += ActivateStartDayButton;
        _dayEndAllBrickDestroyed += PlayEndDayTutorial;

        _dayStart += StartDayTimer;
        _dayStart += DeactivateStartDayButton;

        _onStartBossDay += SwitchBossDayDuration;
        _onStartBossDay += StopDayTimer;
        _onStartBossDay += CheckToSpawnBoss;
        _onStartBossDay += DeactivateStartDayButton;
        //_onEndBossDay += SwitchRegularDayDuration;
        //_onEndBossDay += SetNewEpisode;

        _onBossDefeated += SetBossDefeated;
        //_onBossDefeated += StartDayTimer;

        _towerManager.OnHeightIncrease += MoveBackgroundsY;


        _startDayButton.onClick.AddListener(CheckStartOfDay);

        _daysDuration = _fullDayDuration;
        _currentDayDuration = _daysDuration;

        // Automatically arrange backgrounds vertically.
        ArrangeBackgrounds(_dayBG);
        ArrangeBackgrounds(_nightBG);

        // Continue cycling sprites after the initial backgrounds.
        _nextDayBGIndex = _dayBG.Length;
        _nextNightBGIndex = _nightBG.Length;

        //_currentGameTime = 0f;
        _wasDayTime = IsDayTime();
        AudioManager.Instance.SetMusicArea(MUSIC_TRANSISTION.NIGHT);
    }

    private void OnDisable()
    {
        _onStartBossDay -= SwitchBossDayDuration;
        _onStartBossDay -= StopDayTimer;
        _onStartBossDay -= CheckToSpawnBoss;
        _onStartBossDay -= DeactivateStartDayButton;

        //_onEndBossDay -= SwitchRegularDayDuration;
        //_onEndBossDay -= SetNewEpisode;

        _onBossDefeated -= SetBossDefeated;
        //_onBossDefeated -= StartDayTimer;

        _dayEnd -= PlayDayPassAudio;
        _dayEnd -= _storeAbilityManager.SetItemAvailableToPurchase;
        _dayEnd -= _storeAbilityManager.ResetRoroll;
        _dayEnd -= _overlayUI.ResetItems;
        _dayEnd -= _overlayUI.UpdateRerollText;
        _dayEnd = StopDayTimer;

        _dayEndAllBrickDestroyed -= ActivateStartDayButton;
        _dayEndAllBrickDestroyed -= PlayEndDayTutorial;

        _dayStart -= StartDayTimer;
        _dayStart -= DeactivateStartDayButton;

        _towerManager.OnHeightIncrease -= MoveBackgroundsY;
    }

    private void Update()
    {
        if (!_startDayTimer)
            return;

        CountDayTime();
        _currentRealTimePass += Time.deltaTime;
    }

    public void CountDayTime()
    {
        if (_currentDayDuration > 0f)
        {
            _currentDayDuration -=
                Time.deltaTime * _daySpeedMultiplier;

            if (_currentDayDuration < 0f)
                _currentDayDuration = 0f;

            // -------------------------
            // DAY / NIGHT
            // -------------------------

            bool currentDayTime = IsDayTime();

            if (currentDayTime != _wasDayTime)
            {
                if (currentDayTime)
                {

                    foreach(SpriteRenderer sr in _nightBG)
                    {
                        FadeSpriteAlpha(sr, 0f);
                    }

                    FadeSpriteColor(_frontCloud, _dayFrontCloudShade);
                    FadeSpriteColor(_backCloud, _dayBackCloudShade);
                    AudioManager.Instance.SetMusicArea(MUSIC_TRANSISTION.DAY);
                }
                else
                {
                    foreach (SpriteRenderer sr in _nightBG)
                    {
                        FadeSpriteAlpha(sr, 1f);
                    }
                    FadeSpriteColor(_frontCloud, _nightFrontCloudShade);
                    FadeSpriteColor(_backCloud, _nightBackCloudShade);
                    AudioManager.Instance.SetMusicArea(MUSIC_TRANSISTION.NIGHT);
                }

                _wasDayTime = currentDayTime;
            }
        }
        else
        {
            _totalDayPass++;
            _dayEnd?.Invoke();
            _hasDayEnd = true;


            if (_isBossDay)
            {
                if (!_bossFightEnded)
                {
                    _bossManager._baseBossBrick.GameOverBossEvent();
                }

                _onEndBossDay?.Invoke();

                _bossFightEnded = false;
                _daySpeedMultiplier = 1f;
            }
            else
            {
                if (_totalDayPass >= _maxGameDuration)
                {
                    _endGame?.Invoke();
                    return;
                }

                _currentDayDuration = _daysDuration;
            }
        }
    }
    public void CheckStartOfDay()
    {
        if (_totalDayPass % _bossDayInterval == 0)
        {
            _onStartBossDay?.Invoke();
        }
        else
        {
            _dayStart?.Invoke();
            print("hit");
        }
    }
    public float GetCurrentGameHour()
    {
        float elapsedTime = _daysDuration - _currentDayDuration;

        return (elapsedTime / _daysDuration) * 24f;
    }

    public bool IsDayTime()
    {
        float hour = GetCurrentGameHour();

        return hour >= 6f && hour < 18f;
    }

    public bool IsNightTime()
    {
        return !IsDayTime();
    }
    public void StartDayTimer() => _startDayTimer = true;
    public void StopDayTimer() => _startDayTimer = false;

    public float GetDayNormalized()
    {
        return 1f - (_currentDayDuration / _daysDuration);
    }

    void SwitchBossDayDuration()
    {
        float _timeLeftPercent = GetPercentDayTimeLeft();
        _daysDuration = _bossDayDuration;
        ReSyncTime(_timeLeftPercent);
        _isBossDay = true;
    }

    void SwitchRegularDayDuration()
    {
        float _timeLeftPercent = GetPercentDayTimeLeft();
        _daysDuration = _fullDayDuration;
        ReSyncTime(_timeLeftPercent);
        _isBossDay = false;
    }

    private float GetPercentDayTimeLeft()
    {
        return _currentDayDuration / _daysDuration;
    }

    void ReSyncTime(float percent)
    {
        float timeLeft = _daysDuration * percent;
        _currentDayDuration = timeLeft;
    }

    public void CheckToSpawnBoss()
    {
        if (_isBossDay)
            if (_brickpool.IsAllBrickDestroyed())
            {
                _bossManager.SpawnBoss();
            }
    }

    public void SetBossDefeated()
    {
        _bossFightEnded = true;
        _daySpeedMultiplier = _postBossSpeedMultiplier;
    }

    public float GetCurrentDayDuration() => _currentDayDuration;

    public static void StopTime() => Time.timeScale = 0f;
    public static void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
    public static void SetCustomTimeScale(float val) => Time.timeScale = val;
    public static bool IsGamePause() => Time.timeScale == 0f;

    public float GetCurrentRealTime() => _currentRealTimePass;
    public int GetTotalDayPass() => _totalDayPass;
    public int GetMaxGameDuration() => _maxGameDuration;

    public void PlayDayPassAudio()
    {
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onNewDay, transform.position);
    }
    public void SkipDay() => _currentDayDuration = 1f;

    public bool IsBossDay() => _isBossDay;
    public bool IsEndOfDay() => _hasDayEnd;
    public void FadeSpriteAlpha(SpriteRenderer sprite, float targetAlpha)
    {
        StartCoroutine(FadeSpriteAlphaRoutine(sprite, targetAlpha));
    }
    public void FadeSpriteColor(SpriteRenderer sprite, Color _target)
    {
        StartCoroutine(FadeSpriteColourRoutine(sprite, _target));
    }

    private IEnumerator FadeSpriteAlphaRoutine(
        SpriteRenderer sprite,
        float targetAlpha)
    {
        if (sprite == null)
            yield break;

        Color startColor = sprite.color;
        float startAlpha = startColor.a;

        float timer = 0f;

        while (timer < _backgroundFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                timer / _backgroundFadeDuration);

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t);

            Color newColor = sprite.color;
            newColor.a = alpha;
            sprite.color = newColor;

            yield return null;
        }

        // Ensure final value is exact
        Color finalColor = sprite.color;
        finalColor.a = targetAlpha;
        sprite.color = finalColor;
    }
    private IEnumerator FadeSpriteColourRoutine(
        SpriteRenderer sprite,
        Color target)
    {
        if (sprite == null)
            yield break;

        Color startColor = sprite.color;
        float timer = 0f;

        while (timer < _backgroundFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                timer / _backgroundFadeDuration);

            sprite.color = Color.Lerp(
                startColor,
                target,
                t);

            yield return null;
        }

        // Ensure final colour is exact
        sprite.color = target;
    }
    public void MoveBackgroundsY()
    {
        if (_backgroundMoveCoroutine != null)
            StopCoroutine(_backgroundMoveCoroutine);

        _backgroundMoveCoroutine =
            StartCoroutine(MoveBackgroundsYRoutine());
    }

    private IEnumerator MoveBackgroundsYRoutine()
    {
        float timer = 0f;
        float previousOffset = 0f;

        float duration = Mathf.Max(
            0.001f,
            _backgroundMoveDuration
        );

        while (timer < duration)
        {
            timer = Mathf.Min(
                timer + Time.unscaledDeltaTime,
                duration
            );

            float t = timer / duration;

            // Total movement required at this point in time.
            float currentOffset =
                Mathf.Lerp(0f, _backgroundMoveValue, t);

            // Move only the difference since the previous frame.
            float deltaY = currentOffset - previousOffset;

            previousOffset = currentOffset;

            // Move both background layers.
            MoveBackgroundArray(_dayBG, deltaY);
            MoveBackgroundArray(_nightBG, deltaY);

            // Recycle any backgrounds that have passed the boundary.
            RecycleBackgrounds(
                _dayBG,
                _dayBGList,
                ref _nextDayBGIndex
            );

            RecycleBackgrounds(
                _nightBG,
                _nightBGList,
                ref _nextNightBGIndex
            );

            yield return null;
        }

        _backgroundMoveCoroutine = null;
    }

    private void MoveBackgroundArray(
        SpriteRenderer[] backgrounds,
        float deltaY)
    {
        if (backgrounds == null)
            return;

        Vector3 movement = Vector3.up * deltaY;

        foreach (SpriteRenderer bg in backgrounds)
        {
            if (bg == null)
                continue;

            bg.transform.position += movement;
        }
    }

    private void RecycleBackgrounds(
        SpriteRenderer[] backgrounds,
        List<Sprite> spriteList,
        ref int nextSpriteIndex)
    {
        if (backgrounds == null ||
            backgrounds.Length == 0 ||
            _bgEndPos == null)
            return;

        float endY = _bgEndPos.position.y;

        // Recycle at most one pass through the array.
        // Handles multiple backgrounds reaching the end
        // during the same frame.
        for (int i = 0; i < backgrounds.Length; i++)
        {
            SpriteRenderer lowestBG = null;
            float lowestTop = float.PositiveInfinity;

            // Find the lowest background that has passed
            // the end boundary.
            foreach (SpriteRenderer bg in backgrounds)
            {
                if (bg == null)
                    continue;

                if (bg.bounds.max.y <= endY &&
                    bg.bounds.max.y < lowestTop)
                {
                    lowestBG = bg;
                    lowestTop = bg.bounds.max.y;
                }
            }

            if (lowestBG == null)
                break;

            // Assign the next background sprite.
            if (spriteList != null && spriteList.Count > 0)
            {
                int spriteIndex =
                    nextSpriteIndex % spriteList.Count;

                lowestBG.sprite = spriteList[spriteIndex];
                nextSpriteIndex++;
            }

            // Find the highest point of all OTHER backgrounds.
            float highestY = float.NegativeInfinity;

            foreach (SpriteRenderer bg in backgrounds)
            {
                if (bg == null || bg == lowestBG)
                    continue;

                highestY = Mathf.Max(
                    highestY,
                    bg.bounds.max.y
                );
            }

            // Fallback for an array containing one background.
            if (float.IsNegativeInfinity(highestY))
            {
                highestY = lowestBG.bounds.max.y;
            }

            // Reposition the recycled background
            // directly above the highest background.
            float offsetY =
                highestY - lowestBG.bounds.min.y;

            lowestBG.transform.position +=
                Vector3.up * offsetY;
        }
    }
    private void ArrangeBackgrounds(SpriteRenderer[] backgrounds)
    {
        if (backgrounds == null || backgrounds.Length == 0)
            return;

        SpriteRenderer previousBG = null;

        foreach (SpriteRenderer bg in backgrounds)
        {
            if (bg == null)
                continue;

            if (previousBG != null)
            {
                // Place the bottom edge of this background
                // directly above the previous background.
                float offsetY =
                    previousBG.bounds.max.y - bg.bounds.min.y;

                bg.transform.position += Vector3.up * offsetY;
            }

            previousBG = bg;
        }
    }
    public void ActivateStartDayButton()
    {
        _cardShopButton.SetActive(true);

        if (_firstDay)
            return;
        _startDayButton.gameObject.SetActive(true);

    }
    public void DeactivateStartDayButton()
    {
        _startDayButton.gameObject.SetActive(false);
        _cardShopButton.SetActive(false);
    }

    public void PlayEndDayTutorial()
    {
        if(_firstDay)
        {
            StopTime();
            _tutorialManager.FillSectionOrder(TUTORIAL_TYPE.END_DAY);
            _tutorialManager.StartTutorial();
            _firstDay = false;
            _cardShopButton.gameObject.SetActive(false);
        }

    }
}