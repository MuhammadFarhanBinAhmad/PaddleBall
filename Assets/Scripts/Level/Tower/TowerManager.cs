using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerManager : MonoBehaviour
{

    TimeManager _timeManager;
    public GameObject _collectedBrick;

    [Header("Essence")]
    [Tooltip("Starting threshold of essence required to trigger an OnEssenceCollect.")]
    public int _initialEssenceThreshold;
    [Tooltip("Current threshold; starts at initial and is increased on milestones.")]
    public int _essenceToPureEssenceConversionRate;
    public int _currentEssenceCount;
    internal int _currentPureEssence { get; private set; }
    internal int _totalPureEssenceEarn {  get; private set; }
    internal int _totalPureEssenceSpent { get; private set; }

    [SerializeField] ParticleSystem _onEssenceCollectParticle;
    public Action OnEssenceCollect;

    [Header("Essence scaling (milestones)")]
    public TWEENTYPE _essenceTweenType = TWEENTYPE.LINEAR;
    public int milestoneFloors = 5;
    public int _essenceIncreaseBase = 2;

    [Header("Brick")]
    [SerializeField] int _brickToFloorConversionRate;
    [SerializeField] int _currentBrickCount;

    public Action OnBrickIncrease;
    public Action OnBrickDecrease;
    public Vector2 _posOffset;
    public Vector2 _startPosOffset;

    [Header("Layer")]
    public int _currentTowerHeight;
    [SerializeField]List<GameObject> _createdBricks = new List<GameObject>();
    public Action OnHeightIncrease;
    public Action OnHeightDecrease;

    [Header("Floor")]
    public List<GameObject> _brickList;
    [Header("Floor Animation")]
    [SerializeField] float _floorMoveDuration = 0.3f;
    [SerializeField] AnimationCurve _floorMoveCurve;
    Coroutine _moveRoutine;
    [Header("Feedback")]
    [SerializeField] SO_FeedbackEffect so_OnTowerHit;
    [SerializeField] SO_FeedbackEffect so_OnFloorLost;

    [Header("Tower Health")]
    [SerializeField]int _maxTowerHealth;
    [SerializeField]int _towerHealth;

    [Header("Dailycheck")]
    public TWEENTYPE _towerTweenType = TWEENTYPE.LINEAR;
    public Action _OnGameOver;
    [SerializeField] int _startTowerHeightCheck, _endTowerHeightCheck;
    [SerializeField] int _totalTowerHeightCheck;
    public int[] _towerHeightCheck;
    bool _receiveWarning;

    public Action OnReceivingWarning;
    public Action<int> _onTowerTakingDamage;

    //cheatcode
    public Action OnAddPureEssence;

    void Awake()
    {
        _timeManager = FindAnyObjectByType<TimeManager>();
        _essenceToPureEssenceConversionRate = _initialEssenceThreshold;

    }
    private void Start()
    {
        if (_timeManager != null)
        {
            _timeManager._dayPass += EndOfDayCheck;
            _timeManager._endGame += EndOfGame;
        }

        OnEssenceCollect += IncreaseBrickCount;

        OnHeightIncrease += CreateNewFloor;


        _onTowerTakingDamage += TowerTakeDamage;

        OnAddPureEssence += AddPureEssence;

        OnReceivingWarning += WarningGiven;

        PopulateTowerHeightCheck();

        _towerHealth = _maxTowerHealth;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        // ensure array is updated in editor without entering playmode
        PopulateTowerHeightCheck();
    }
#endif
    private void OnDisable()
    {
        _timeManager._dayPass -= EndOfDayCheck;
        
        _timeManager._endGame -= EndOfGame;
        
        OnEssenceCollect -= IncreaseBrickCount;
        
        OnHeightIncrease -= CreateNewFloor;

        _onTowerTakingDamage -= TowerTakeDamage;

        OnAddPureEssence -= AddPureEssence;

        OnReceivingWarning -= WarningGiven;

    }

    public void IncreaseEssenceCount(int amt)
    {
        _currentEssenceCount += amt;
        _onEssenceCollectParticle.Play();
        if (_currentEssenceCount >= _essenceToPureEssenceConversionRate)
        {
            _currentEssenceCount = 0;
            _currentBrickCount++;
            _currentPureEssence++;
            _totalPureEssenceEarn++;
            GameObject brick = Instantiate(_collectedBrick);
            Vector3 pos = new Vector2((transform.position.x + _startPosOffset.x) + (transform.position.x / 2) + (_posOffset.x * _currentBrickCount), transform.position.y);
            brick.transform.position = pos;
            _createdBricks.Add(brick);
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBrickMade, transform.position);
        }
        OnEssenceCollect?.Invoke();
    }
    public void IncreaseBrickCount()
    {

        OnBrickIncrease?.Invoke();

        if (_currentBrickCount >= _brickToFloorConversionRate)
        {
            _currentBrickCount = 0;
            _currentTowerHeight++;
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onFloorMade, transform.position);

            // Apply milestone logic: every milestoneFloors floors, increase essence threshold
            if (milestoneFloors > 0 && _currentTowerHeight % milestoneFloors == 0)
            {
                ApplyEssenceMilestoneIncrease();
            }

            OnHeightIncrease?.Invoke();
        }
    }

    void ApplyEssenceMilestoneIncrease()
    {
        // compute which milestone we are at (1-based)
        int milestoneIndex = Mathf.FloorToInt((float)_currentTowerHeight / milestoneFloors);

        // use TimeManager's max month as a normaliser to build progress for the easing curve
        int maxPhases;
        maxPhases = _timeManager.GetTotalDayPass();

        // progress in [0,1] = milestoneIndex / maxPhases (clamped)
        float progress = (float)milestoneIndex / (float)maxPhases;
        progress = Mathf.Clamp01(progress);

        float eased = TweenService.GetEased(progress, _essenceTweenType);

        // compute increase (at least 1)
        int increase = Mathf.Max(1, Mathf.RoundToInt(_essenceIncreaseBase * eased));

        _essenceToPureEssenceConversionRate += increase;
    }
    void WarningGiven()
    {
        _receiveWarning = true;
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onFirstWarning,transform.position);
    }

    void CreateNewFloor()
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(AnimateShiftDown());
    }

    public void TowerTakeDamage(int layer)
    {
        RemoveBrick();
        GlobalFeedbackManager.Instance.SetFeedbackValue(so_OnTowerHit);
        if (_currentBrickCount <= -1)
        {
            RemoveFloor();
            OnHeightDecrease?.Invoke();
            GlobalFeedbackManager.Instance.SetFeedbackValue(so_OnFloorLost);
        }
        GlobalFeedbackManager.Instance.PlayGlobalFeedback();
    }
    void RemoveBrick()
    {
        if(_createdBricks.Count > 0)
        {
            _currentBrickCount--;
            GameObject brick = _createdBricks[_createdBricks.Count - 1];
            _createdBricks.RemoveAt(_createdBricks.Count - 1);
            Destroy(brick);
        }
    }
    void RemoveFloor()
    {
        if (_currentTowerHeight >0 )
        {
            _currentTowerHeight--;
            _currentBrickCount = _brickToFloorConversionRate - 1;
            if (_moveRoutine != null)
                StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(AnimateShiftUp());
        }
    }
    IEnumerator AnimateShiftDown()
    {
        if (_createdBricks.Count == 0)
        {
            _moveRoutine = null;
            yield break;
        }

        float time = 0f;

        List<GameObject> bricksSnapshot = new List<GameObject>(_createdBricks);

        int count = bricksSnapshot.Count;
        Vector3[] startPos = new Vector3[count];
        Vector3[] targetPos = new Vector3[count];

        float shiftAmount = _posOffset.y;

        for (int i = 0; i < count; i++)
        {
            if (bricksSnapshot[i] == null) continue;

            startPos[i] = bricksSnapshot[i].transform.position;
            targetPos[i] = startPos[i] - new Vector3(0f, shiftAmount, 0f);
        }

        while (time < _floorMoveDuration)
        {
            if (_createdBricks.Count == 0)
            {
                _moveRoutine = null;
                yield break;
            }

            float t = time / _floorMoveDuration;
            float curveT = _floorMoveCurve != null ? _floorMoveCurve.Evaluate(t) : t;

            for (int i = 0; i < count; i++)
            {
                if (bricksSnapshot[i] == null) continue;

                bricksSnapshot[i].transform.position =
                    Vector3.Lerp(startPos[i], targetPos[i], curveT);
            }

            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            if (bricksSnapshot[i] == null) continue;
            bricksSnapshot[i].transform.position = targetPos[i];
        }

        _moveRoutine = null;
    }
    IEnumerator AnimateShiftUp()
    {
        if (_createdBricks.Count == 0)
        {
            _moveRoutine = null;
            yield break;
        }

        float time = 0f;

        // Snapshot the current bricks
        List<GameObject> bricksSnapshot = new List<GameObject>(_createdBricks);

        int count = bricksSnapshot.Count;
        Vector3[] startPos = new Vector3[count];
        Vector3[] targetPos = new Vector3[count];

        float shiftAmount = _posOffset.y;

        for (int i = 0; i < count; i++)
        {
            if (bricksSnapshot[i] == null) continue;

            startPos[i] = bricksSnapshot[i].transform.position;
            targetPos[i] = startPos[i] + new Vector3(0f, shiftAmount, 0f);
        }

        while (time < _floorMoveDuration)
        {
            if (_createdBricks.Count == 0)
            {
                _moveRoutine = null;
                yield break;
            }

            float t = time / _floorMoveDuration;
            float curveT = _floorMoveCurve != null ? _floorMoveCurve.Evaluate(t) : t;

            for (int i = 0; i < count; i++)
            {
                if (bricksSnapshot[i] == null) continue;

                bricksSnapshot[i].transform.position =
                    Vector3.Lerp(startPos[i], targetPos[i], curveT);
            }

            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < count; i++)
        {
            if (bricksSnapshot[i] == null) continue;
            bricksSnapshot[i].transform.position = targetPos[i];
        }

        _moveRoutine = null;
    }
    public void EndOfDayCheck()
    {
        //if(_currentTowerHeight >= _towerHeightCheck[_timeManager.GetTotalDayPass()])
        //{
        //    print("pass");
        //}
        //else
        //{
        //    if (!_receiveWarning)
        //    {
        //        OnReceivingWarning?.Invoke();
        //    }
        //    else
        //    {
        //        _OnGameOver?.Invoke();
        //        TimeManager.StopTime();
        //        print("fail");
        //    }
        //}
    }
    public void EndOfGame()
    {
        print("end game");
        //TimeManager.StopTime();
    }
    void PopulateTowerHeightCheck()
    {
        if (_totalTowerHeightCheck <= 0)
        {
            _towerHeightCheck = new int[0];
            return;
        }

        _towerHeightCheck = new int[_totalTowerHeightCheck];

        // if only one sample, use start value
        if (_totalTowerHeightCheck == 1)
        {
            _towerHeightCheck[0] = _startTowerHeightCheck;
            return;
        }

        int steps = _totalTowerHeightCheck - 1; // denom so last element = end value
        for (int i = 0; i < _totalTowerHeightCheck; i++)
        {
            float t = (float)i / (float)steps;               // normalized [0,1]
            float eased = TweenService.GetEased(t, _towerTweenType);      // apply chosen easing
            float val = Mathf.Lerp(_startTowerHeightCheck, _endTowerHeightCheck, eased);
            _towerHeightCheck[i] = Mathf.RoundToInt(val);    // integer thresholds
        }
    }

    public int GetCurrentEssence() => _currentEssenceCount;
    public void DeductPureEssence(int value)
    {
        _currentPureEssence -= value;
        _totalPureEssenceSpent += value;
    }
    public int GetTotalPureEssenceCount() => _currentPureEssence;
    public int GetCurrentBrickCount() => _currentBrickCount;
    public int GetBrickFloorConversionRate() => _brickToFloorConversionRate;
    public int GetEssencePureEssenceConversionRate() => _essenceToPureEssenceConversionRate;
    public void AddPureEssence() => _currentPureEssence++;

}
