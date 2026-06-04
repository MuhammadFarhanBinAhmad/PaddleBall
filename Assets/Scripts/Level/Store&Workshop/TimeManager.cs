using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    StoreAbilityManager _storeAbilityManager;

    [Header("TimeKeeper")]
    [SerializeField] int _maxGameDuration;
    int _totalDayPass;
    [SerializeField] float _fullDayDuration;
    [SerializeField] float _currentDayDuration;

    public Action _dayPass;
    public Action _endGame;

    [Header("RealTime")]
    [SerializeField] float _currentRealTimePass;

    private void Awake()
    {
        _storeAbilityManager = FindAnyObjectByType<StoreAbilityManager>();
    }
    private void Start()
    {
        _currentDayDuration = _fullDayDuration;
        _dayPass += PlayDayPassAudio;
        _dayPass += _storeAbilityManager.RerollItem;
        _dayPass += _storeAbilityManager.ResetRoroll;
    }
    private void OnDisable()
    {
        _dayPass -= PlayDayPassAudio;
        _dayPass -= _storeAbilityManager.RerollItem;
        _dayPass -= _storeAbilityManager.ResetRoroll;
    }
    private void Update()
    {
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
            _currentDayDuration = _fullDayDuration;
            _dayPass?.Invoke();
            
        }
    }
    public float GetDayNormalized()
    {
        return 1f - (_currentDayDuration / _fullDayDuration);
    }
    public static void StopTime() => Time.timeScale = 0f;
    public static void ResetTimeScale() => Time.timeScale = 1f;
    public static void SetCustomTimeScale(float val) => Time.timeScale = val;
    public static bool IsGamePause() => Time.timeScale == 0f;
    public float GetCurrentRealTime () => _currentRealTimePass;
    public int GetTotalDayPass() => _totalDayPass;
    public int GetMaxGameDuration() => _maxGameDuration;
    public void PlayDayPassAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onNewDay, transform.position);
    

}
