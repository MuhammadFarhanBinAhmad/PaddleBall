using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossAttackManager : MonoBehaviour
{
    internal enum AttackType
    {
        One,
        Two,
        Three
    }

    internal enum AttackEndMode
    {
        None,
        HitCount,
        Duration,
        Point,
        Manual
    }


    [Header("Boss Attack Flow")]
    [SerializeField] private float _restDuration;

    [Header("Stun")]
    [SerializeField] private float _stunDuration;

    [Header("Ping Pong Movement")]
    [SerializeField] internal List<Transform> _movementPoints = new List<Transform>();
    [SerializeField] internal float _movementSpeed;
    [SerializeField] internal float _arrivalThreshold;

    internal Coroutine _movementRoutine;
    internal int _currentMovementTargetIndex;
    internal int _movementDirection = 1;
    internal Transform _currentPointTarget;

    internal bool _stopAttacking;
    internal bool _isStunned;

    internal AttackEndMode _currentAttackEndMode = AttackEndMode.None;
    internal int _targetHits;
    internal int _currentHits;
    internal float _attackEndTime;
    internal bool _attackComplete;
    protected bool IsAttackActive => !_attackComplete;

    Coroutine _bossRoutine;

    private readonly List<AttackType> _attackOrder = new List<AttackType>();


    public virtual void StartBossFight()
    {
        _bossRoutine = StartCoroutine(BossAttackLoop());
    }

    private IEnumerator BossAttackLoop()
    {
        while (!_stopAttacking)
        {
            BuildAndShuffleAttackList();

            yield return RestSequence();
            if (_stopAttacking) yield break;

            for (int i = 0; i < _attackOrder.Count; i++)
            {
                if (_stopAttacking)
                    yield break;

                while (_isStunned && !_stopAttacking)
                    yield return null;

                if (_stopAttacking)
                    yield break;

                yield return ExecuteAttack(_attackOrder[i]);

                while (_isStunned && !_stopAttacking)
                    yield return null;

                if (_stopAttacking)
                    yield break;

                yield return RestSequence();
            }
        }
    }

    private void BuildAndShuffleAttackList()
    {
        _attackOrder.Clear();
        _attackOrder.Add(AttackType.One);
        _attackOrder.Add(AttackType.Two);
        _attackOrder.Add(AttackType.Three);

        ShuffleList(_attackOrder);
    }

    private void ShuffleList(List<AttackType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private IEnumerator RestSequence()
    {
        StopPingPongMovement();
        RestToNeutral();
        yield return new WaitForSeconds(_restDuration);
    }
    
    private IEnumerator ExecuteAttack(AttackType attack)
    {
        ResetCurrentAttackState();

        switch (attack)
        {
            case AttackType.One:
                AttackPatternOne();
                break;

            case AttackType.Two:
                AttackPatternTwo();
                break;

            case AttackType.Three:
                AttackPatternThree();
                break;
        }

        yield return WaitForCurrentAttackToFinish();
    }

    private IEnumerator WaitForCurrentAttackToFinish()
    {
        while (!_stopAttacking && !_isStunned && !_attackComplete)
        {
            if (_currentAttackEndMode == AttackEndMode.Duration &&
                Time.time >= _attackEndTime)
            {
                _attackComplete = true;
                break;
            }

            yield return null;
        }

        StopPingPongMovement();
    }

    protected void BeginHitCountAttack(int hitsNeeded)
    {
        _currentAttackEndMode = AttackEndMode.HitCount;
        _targetHits = Mathf.Max(1, hitsNeeded);
        _currentHits = 0;
        _attackComplete = false;
    }

    protected void BeginManualAttack()
    {
        _currentAttackEndMode = AttackEndMode.Manual;
        _attackComplete = false;
    }

    protected void RegisterAttackHit(int amount = 1)
    {
        if (_currentAttackEndMode != AttackEndMode.HitCount || _attackComplete)
            return;

        _currentHits += amount;
        if (_currentHits >= _targetHits)
            _attackComplete = true;
    }

    protected void CompleteCurrentAttack()
    {
        _attackComplete = true;
    }

    protected void ResetCurrentAttackState()
    {
        _currentAttackEndMode = AttackEndMode.None;
        _targetHits = 0;
        _currentHits = 0;
        _attackEndTime = 0f;
        _attackComplete = false;
    }

    protected void StopPingPongMovement()
    {
        if (_movementRoutine != null)
        {
            StopCoroutine(_movementRoutine);
            _movementRoutine = null;
        }
    }

    public void StunBoss(float duration)
    {

        _stunDuration = duration;
        if (_stopAttacking || _isStunned)
            return;

        if (_bossRoutine != null)
            StopCoroutine(_bossRoutine);

        StartCoroutine(StunRoutine());
    }

    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        CompleteCurrentAttack();
        StopPingPongMovement();
        RestToNeutral();

        Debug.Log("IsStun");

        yield return new WaitForSeconds(_stunDuration);

        Debug.Log("FinishStun");

        _isStunned = false;

        if (!_stopAttacking)
            _bossRoutine = StartCoroutine(BossAttackLoop());
    }


    public virtual void StopBossAttack()
    {
        _stopAttacking = true;
        _bossRoutine = null;
        StopAllCoroutines();
    }
    public void SetSpeed(float speed) => _movementSpeed = speed; 
    public abstract void AttackPatternOne();
    public abstract void AttackPatternTwo();
    public abstract void AttackPatternThree();
    public abstract void RestToNeutral();
}