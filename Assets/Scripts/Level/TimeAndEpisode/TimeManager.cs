using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimeManager : MonoBehaviour
{
    BrickPool _brickpool;
    StoreAbilityManager _storeAbilityManager;
    BossManager _bossManager;
    EpisodeManager _episodeManager;

    [Header("TimeKeeper")]
    [SerializeField] int _maxGameDuration;
    int _totalDayPass;
    [SerializeField] float _fullDayDuration;
    [SerializeField] float _bossDayDuration;
    float _daysDuration;
    [SerializeField] float _currentDayDuration;
    [SerializeField] int _bossDayInterval;

    bool _isBossDay;
    bool _bossFightEnded;

    [Header("Timelapse")]
    [SerializeField] float _postBossSpeedMultiplier = 6f;
    float _daySpeedMultiplier = 1f;

    public Action _onStartBossDay;
    public Action _onEndBossDay;
    public Action _onBossDefeated;

    public Action _dayPass;
    public Action _endGame;

    [Header("Game Clock")]
    //[SerializeField] float _currentGameTime;

    [Header("RealTime")]
    [SerializeField] float _currentRealTimePass;

    [Header("BackGround")]
    [SerializeField] float _backgroundFadeDuration = 1f;
    [SerializeField] SpriteRenderer _dayBG;
    [SerializeField] SpriteRenderer _nightBG;
    [SerializeField] SpriteRenderer _frontCloud,_backCloud;
    [SerializeField] Color _dayFrontCloudShade, _nightFrontCloudShade;
    [SerializeField] Color _dayBackCloudShade, _nightBackCloudShade;

    bool _startDayTimer;
    bool _wasDayTime;

    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _bossManager = FindAnyObjectByType<BossManager>();
        _brickpool = FindAnyObjectByType<BrickPool>();
        _episodeManager = FindAnyObjectByType<EpisodeManager>();
    }

    private void Start()
    {
        _dayPass += PlayDayPassAudio;
        _dayPass += _storeAbilityManager.RerollItem;
        _dayPass += _storeAbilityManager.ResetRoroll;

        _onStartBossDay += SwitchBossDayDuration;
        _onStartBossDay += StopDayTimer;
        _onStartBossDay += CheckToSpawnBoss;

        _onEndBossDay += SwitchRegularDayDuration;
        _onEndBossDay += SetNewEpisode;

        _onBossDefeated += SetBossDefeated;
        _onBossDefeated += StartDayTimer;

        _daysDuration = _fullDayDuration;
        _currentDayDuration = _daysDuration;

        //_currentGameTime = 0f;
        _wasDayTime = IsDayTime();
        AudioManager.Instance.SetMusicArea(MUSIC_TRANSISTION.NIGHT);
    }

    private void OnDisable()
    {
        _onStartBossDay -= SwitchBossDayDuration;
        _onStartBossDay -= StopDayTimer;
        _onStartBossDay -= CheckToSpawnBoss;

        _onEndBossDay -= SwitchRegularDayDuration;
        _onEndBossDay -= SetNewEpisode;

        _onBossDefeated -= SetBossDefeated;
        _onBossDefeated -= StartDayTimer;

        _dayPass -= PlayDayPassAudio;
        _dayPass -= _storeAbilityManager.RerollItem;
        _dayPass -= _storeAbilityManager.ResetRoroll;
    }

    private void Update()
    {
        if (!_startDayTimer)
            return;

        CountDayTime();

        // Always progresses according to the regular 24-hour cycle
        //_currentGameTime += Time.deltaTime * _daySpeedMultiplier;

        //if (_currentGameTime >= _fullDayDuration)
        //    _currentGameTime -= _fullDayDuration;

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
                    Debug.Log("06:00 - DAY TIME");

                    FadeSpriteAlpha(_nightBG, 0f);
                    FadeSpriteColor(_frontCloud, _dayFrontCloudShade);
                    FadeSpriteColor(_backCloud, _dayBackCloudShade);
                    AudioManager.Instance.SetMusicArea(MUSIC_TRANSISTION.DAY);
                }
                else
                {
                    Debug.Log("18:00 - NIGHT TIME");

                    FadeSpriteAlpha(_nightBG, 1f);
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

                if (_totalDayPass % _bossDayInterval == 0)
                {
                    _onStartBossDay?.Invoke();
                }

                _currentDayDuration = _daysDuration;

                _dayPass?.Invoke();
            }
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
    void SetNewEpisode()
    {
        int index = Random.Range(0,_episodeManager.GetTotalEpisode());
        _episodeManager.OnStartEpisode?.Invoke(index);
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
        print("HIT");
        if (_isBossDay)
            if (_brickpool.IsAllBrickDestroyed())
                _bossManager.SpawnBoss();
    }

    public void SetBossDefeated()
    {
        _bossFightEnded = true;
        _daySpeedMultiplier = _postBossSpeedMultiplier;
    }

    public bool IsBossDay() => _isBossDay;
    public bool IsBossFightEnded() => _bossFightEnded;
    public float GetCurrentDayDuration() => _currentDayDuration;

    public static void StopTime() => Time.timeScale = 0f;
    public static void ResetTimeScale() => Time.timeScale = 1f;
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
}