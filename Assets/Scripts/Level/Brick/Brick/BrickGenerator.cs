using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum BRICKLAYER
{
    NONE,
    RED,
    GREEN,
    BLUE,
    ORANGE,
    PINK,
    PURPLE,
    YELLOW,
    BLACK,
    WHITE
}

public class PlannedBrick
{
    public SO_BrickHealthStats stats;
    public Vector3 position;
}
public class WavePlan
{
    public List<PlannedBrick> bricks = new List<PlannedBrick>();
    public int totalAPUsed;
}

[Serializable]
public class BrickFormationEntry
{
    public List<SOBrickFormation> formations;
}

public class BrickGenerator : MonoBehaviour
{
    BrickPool _brickPool;
    TimeManager _timeManager;
    BrickModifierList _brickModifierList;

    public List<BrickFormationEntry> _brickFormationList = new List<BrickFormationEntry>();

    public List<SO_BrickHealthStats> _brickTypesList;
    public List<SO_BrickHealthStats> _brickAvailableToSpawn = new List<SO_BrickHealthStats>();

    [Header("AttributePoints")]
    TWEENTYPE _APTweenType;
    [SerializeField] int _firstAttributePoints;
    [SerializeField] int _lastAttributePoints;
    public int[] _attributePoints;
    int _APPerWaveForTheDay;

    [Header("Brick position")]
    public Vector2Int _size;
    public Vector2 _offset;

    [Header("BrickSpawn")]
    [SerializeField] AnimationCurve easeOutElastic;
    [SerializeField] float animationDuration;
    [SerializeField] float _capscaleMultiplier;
    Vector3 _startingScale = new Vector3(1,1,1);

    [Header("Level and Wave")]
    public List<int> _spawnedWaves = new List<int>();
    public GameObject _brickPrefab;
    [SerializeField] float _timerBeforeNextLineSpawn;
    public int _brickCounter;
    public int _currentWave;
    int _currentWaveAP;
    bool _stopWaveSpawn;

    [Header("Timer before next wave spawn")]
    [SerializeField] float _timerBeforeNextWaveSpawn;
    public Action _onSpawnNextWave;

    [Header("Boss Spawner")]
    [SerializeField] SO_BossBrickStats _starterBoss;
    [SerializeField] List<SO_BossBrickStats> _BossList = new List<SO_BossBrickStats>();
    public Action _onSpawnBoss;

    private void Awake()
    {
        _brickPool = GetComponent<BrickPool>();
        _timeManager = FindAnyObjectByType<TimeManager>();
        _brickModifierList = GetComponent<BrickModifierList>();
    }

    private void Start()
    {
        _timeManager._dayPass += CheckBrickToAdd;
        _timeManager._dayPass += _brickModifierList.CheckModifierToAdd;
        _timeManager._dayPass += SetAPOfTheDay;

        _timeManager._onStartBossDay += StopWaveSpawning;

        _onSpawnNextWave += SpawnNextWave;

        SetAttributePointForEachPhase();
        _brickModifierList.PopulateModifierChanceTable();
        CheckBrickToAdd();
        _brickModifierList.CheckModifierToAdd();
        SetAPOfTheDay();
    }
    private void OnDisable()
    {
        _onSpawnNextWave -= SpawnNextWave;
        _timeManager._dayPass -= CheckBrickToAdd;
        _timeManager._dayPass -= SetAPOfTheDay;

        _timeManager._onStartBossDay -= StopWaveSpawning;
    }

    public void OnBrickDestroyed()
    {
        _brickCounter--;
    }

    public void StartFirstWaveOfEpisode()
    {
        _stopWaveSpawn = false;
        _timeManager.StartDayTimer();
        _onSpawnNextWave?.Invoke();
    }
    public void StopWaveSpawning()
    {
        _stopWaveSpawn = true;
    }
    public SOBrickFormation GetBrickFormation()
    {
        var formations = _brickFormationList[0].formations;

        if (formations == null || formations.Count == 0)
        {
            Debug.LogWarning($"Brick formation list is empty for level {0}");
            return null;
        }

        // Fast lookup of already spawned indices
        var used = new HashSet<int>(_spawnedWaves);

        // Build list of available indices
        var available = new List<int>(formations.Count);
        for (int i = 0; i < formations.Count; i++)
        {
            if (!used.Contains(i))
                available.Add(i);
        }

        // Pick a random index from the remaining ones
        int pick = available[UnityEngine.Random.Range(0, available.Count)];
        _spawnedWaves.Add(pick);

        return formations[pick];
    }

    void SetAttributePointForEachPhase()
    {
        int phases = 1 ;
        if (_timeManager != null)
            phases = _timeManager.GetMaxGameDuration() ;
        else
            Debug.LogWarning("TimeManager not found when generating health per phase. Defaulting to 1 phase.");

        // Ensure arrays have correct size
        _attributePoints = new int[phases];

        for (int i = 0; i < phases; i++)
        {
            float tStart = (phases == 1) ? 0f : (float)i / (phases - 1);

            float easedStart = TweenService.GetEased(tStart, _APTweenType);

            float val = Mathf.Lerp(_firstAttributePoints, _lastAttributePoints, easedStart);

            _attributePoints[i] = Mathf.RoundToInt(val);
        }
    }
    void SetAPOfTheDay()
    {
        if(_timeManager.GetMaxGameDuration() > _timeManager.GetTotalDayPass())
            _APPerWaveForTheDay = _attributePoints[_timeManager.GetTotalDayPass()];
    }
    void SpawnNextWave()
    {
        WavePlan plan = BuildWavePlan(GetBrickFormation());
        StartCoroutine(ExecuteWavePlan(plan));

        if (_currentWave >= _brickFormationList[0].formations.Count - 1)
        {
            _currentWave = 0;
            _spawnedWaves.Clear();
        }
        else
        {
            _currentWave++;
        }
    }

    IEnumerator ExecuteWavePlan(WavePlan plan)
    {
        foreach (var p in plan.bricks)
        {
            GameObject brick = _brickPool.GetBrick();
            _brickPool.PlaceActiveBrickInList(brick);

            BrickBar bb = brick.GetComponent<BrickBar>();

            brick.transform.position = p.position;
            brick.transform.localScale = _startingScale;

            bb.SetBrick(p.stats);
            if(_timeManager.GetTotalDayPass() >= _brickModifierList._dayFirstModiferCheckUnlock)
            {
                if (_brickModifierList.RollForModifier(true))
                {
                    _brickModifierList.TryAddRandomModifier(bb, _brickModifierList.RollRarity());
                    if (_brickModifierList.RollForModifier(false))
                        _brickModifierList.TryAddRandomModifier(bb, _brickModifierList.RollRarity());
                }
            }


            _brickCounter++;

            StartCoroutine(AnimateBrickSpawn(brick.transform));

            yield return null;
        }

        yield return new WaitForSeconds(_timerBeforeNextWaveSpawn);

        if(!_stopWaveSpawn)
        _onSpawnNextWave?.Invoke();
    }
    WavePlan BuildWavePlan(SOBrickFormation formation)
    {
        WavePlan plan = new WavePlan();

        int ap = _APPerWaveForTheDay;
        int x = 0;
        int y = 0;

        foreach (char c in formation.formation)
        {
            if (c == '\n')
            {
                y++;
                x = 0;
                continue;
            }

            if (c == '0')
            {
                x++;
                continue;
            }

            if (c == '1')
            {
                x++;

                var available = GetAffordableBricks(ap);

                if (available.Count == 0)
                    continue;

                var stats = available[UnityEngine.Random.Range(0, available.Count)];

                Vector3 pos =
                    transform.position +
                    new Vector3(_offset.x * (x + 0.5f), _offset.y * (y + 0.5f));

                plan.bricks.Add(new PlannedBrick
                {
                    stats = stats,
                    position = pos
                });

                ap -= stats._APValue;
            }
        }

        plan.totalAPUsed = _APPerWaveForTheDay - ap;

        return plan;
    }
    public void CheckBrickToAdd()
    {
        int day = _timeManager.GetTotalDayPass();

        for (int i = 0; i < _brickTypesList.Count; i++)
        {
            // 1. Skip if already unlocked
            if (_brickAvailableToSpawn.Contains(_brickTypesList[i]))
                continue;

            // 2. Unlock if day matches
            if (_brickTypesList[i]._daytoUnlock == day)
            {
                _brickAvailableToSpawn.Add(_brickTypesList[i]);
            }
        }
    }

    public void SpawnBoss()
    {
    }

    IEnumerator AnimateBrickSpawn(Transform brickTransform)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = _startingScale * _capscaleMultiplier;

        float time = 0f;

        while (time < animationDuration)
        {
            float normalized = time / animationDuration;
            float curveValue = easeOutElastic.Evaluate(normalized);

            brickTransform.localScale =
                Vector3.LerpUnclamped(startScale, targetScale, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        brickTransform.localScale = _startingScale;
    }

    List<SO_BrickHealthStats> GetAffordableBricks(int ap)
    {
        List<SO_BrickHealthStats> result = new List<SO_BrickHealthStats>();

        for (int i = 0; i < _brickAvailableToSpawn.Count; i++)
        {
            if (_brickAvailableToSpawn[i]._APValue <= ap)
                result.Add(_brickAvailableToSpawn[i]);
        }

        return result;
    }
   
}
