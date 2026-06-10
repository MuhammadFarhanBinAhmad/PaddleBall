using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    StoreAbilityManager _storeAbilityManager;
    EpisodeManager _episodeManager;

    [Header("TimeKeeper")]
    [SerializeField] int _maxGameDuration;
    int _totalDayPass;
    [SerializeField] float _fullDayDuration;
    [SerializeField] float _bossDayDuration;
    float _daysDuration;
    [SerializeField] float _currentDayDuration;
    [SerializeField] int _bossDayInterval;


    public Action _onStartBossDay;
    public Action _onEndBossDay;
    public Action _dayPass;
    public Action _endGame;

    [Header("RealTime")]
    [SerializeField] float _currentRealTimePass;

    bool _startDayTimer;

    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
        _episodeManager = FindAnyObjectByType<EpisodeManager>();
    }
    private void Start()
    {
        _dayPass += PlayDayPassAudio;
        _dayPass += _storeAbilityManager.RerollItem;
        _dayPass += _storeAbilityManager.ResetRoroll;

        _onStartBossDay += SwitchBossDayDuration;
        _onStartBossDay += StopDayTimer;

        _onEndBossDay += SwitchRegularDayDuration;
        _onEndBossDay += StopDayTimer;

        _daysDuration = _fullDayDuration;
        _currentDayDuration = _daysDuration;

    }
    private void OnDisable()
    {
        _onStartBossDay -= SwitchBossDayDuration;
        _onStartBossDay -= StopDayTimer;

        _onEndBossDay -= SwitchRegularDayDuration;
        _onEndBossDay -= StopDayTimer;

        _dayPass -= PlayDayPassAudio;
        _dayPass -= _storeAbilityManager.RerollItem;
        _dayPass -= _storeAbilityManager.ResetRoroll;
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
        if(_currentDayDuration > 0)
        {
            _currentDayDuration -= Time.deltaTime;
        }
        else
        {
            _totalDayPass++;
            if(_totalDayPass >= _maxGameDuration)
            {
                print("end of game");
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
    public void StartDayTimer() => _startDayTimer = true;
    public void StopDayTimer() => _startDayTimer = false;

    public float GetDayNormalized()
    {
        return 1f - (_currentDayDuration / _daysDuration);
    }
    void SwitchBossDayDuration() => _daysDuration = _bossDayDuration;
    void SwitchRegularDayDuration() => _daysDuration = _fullDayDuration;

    public static void StopTime() => Time.timeScale = 0f;
    public static void ResetTimeScale() => Time.timeScale = 1f;
    public static void SetCustomTimeScale(float val) => Time.timeScale = val;
    public static bool IsGamePause() => Time.timeScale == 0f;
    public float GetCurrentRealTime () => _currentRealTimePass;
    public int GetTotalDayPass() => _totalDayPass;
    public int GetMaxGameDuration() => _maxGameDuration;
    public void PlayDayPassAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onNewDay, transform.position);
    

}
