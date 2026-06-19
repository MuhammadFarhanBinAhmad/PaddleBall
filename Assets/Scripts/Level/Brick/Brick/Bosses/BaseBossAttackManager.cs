using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossAttackManager : MonoBehaviour
{
    private enum AttackType
    {
        One,
        Two,
        Three
    }

    private enum AttackEndMode
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
    [SerializeField] private List<Transform> _movementPoints = new List<Transform>();
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _arrivalThreshold;

    private Coroutine _movementRoutine;
    private int _currentMovementTargetIndex;
    private int _movementDirection = 1;
    private Transform _currentPointTarget;

    private bool _stopAttacking;
    private bool _isStunned;

    private AttackEndMode _currentAttackEndMode = AttackEndMode.None;
    private int _targetHits;
    private int _currentHits;
    private float _attackEndTime;
    private bool _attackComplete;
    protected bool IsAttackActive => !_attackComplete;

    private Coroutine _bossRoutine;

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
    private IEnumerator PointMovementRoutine()
    {
        _currentPointTarget = _movementPoints[_currentMovementTargetIndex];

        while (!_stopAttacking && !_isStunned && !_attackComplete)
        {
            if (_currentPointTarget == null)
            {
                CompleteCurrentAttack();
                yield break;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                _currentPointTarget.position,
                _movementSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, _currentPointTarget.position) <= _arrivalThreshold)
            {
                AdvanceMovementPoint();
                CompleteCurrentAttack();
                break;
            }

            yield return null;
        }

        _movementRoutine = null;
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
    private void AdvanceMovementPoint()
    {
        _currentMovementTargetIndex += _movementDirection;

        if (_currentMovementTargetIndex >= _movementPoints.Count)
        {
            _movementDirection = -1;
            _currentMovementTargetIndex = _movementPoints.Count - 2;
        }
        else if (_currentMovementTargetIndex < 0)
        {
            _movementDirection = 1;
            _currentMovementTargetIndex = 1;
        }
    }
    protected void BeginHitCountAttack(int hitsNeeded)
    {
        _currentAttackEndMode = AttackEndMode.HitCount;
        _targetHits = Mathf.Max(1, hitsNeeded);
        _currentHits = 0;
        _attackComplete = false;
    }

    protected void BeginTimedAttack(float duration)
    {
        _currentAttackEndMode = AttackEndMode.Duration;
        _attackEndTime = Time.time + Mathf.Max(0f, duration);
        _attackComplete = false;
    }
    protected void BeginPointAttack()
    {
        _currentAttackEndMode = AttackEndMode.Point;
        _attackComplete = false;
        if (_movementPoints == null || _movementPoints.Count < 2)
        {
            Debug.LogWarning($"{name}: Not enough movement points for point attack.");
            CompleteCurrentAttack();
            return;
        }

        StopPingPongMovement();
        _movementRoutine = StartCoroutine(PointMovementRoutine());
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

    protected void BeginPingPongMovement(bool snapToFirstPoint = true)
    {
        if (_movementPoints == null || _movementPoints.Count < 2)
        {
            Debug.LogWarning($"{name}: Not enough movement points for ping-pong movement.");
            return;
        }

        StopPingPongMovement();

        //if (snapToFirstPoint)
        //{
        //    transform.position = _movementPoints[0].position;
        //    _currentMovementTargetIndex = 1;
        //    _movementDirection = 1;
        //}
        //else
        //{
        //    _currentMovementTargetIndex = Mathf.Clamp(_currentMovementTargetIndex, 0, _movementPoints.Count - 1);
        //}


        _movementRoutine = StartCoroutine(PingPongMovementRoutine());
    }

    protected void StopPingPongMovement()
    {
        if (_movementRoutine != null)
        {
            StopCoroutine(_movementRoutine);
            _movementRoutine = null;
        }
    }

    private IEnumerator PingPongMovementRoutine()
    {
        while (!_stopAttacking && !_isStunned && !_attackComplete)
        {
            if (_movementPoints == null || _movementPoints.Count < 2)
                yield break;

            Transform target = _movementPoints[_currentMovementTargetIndex];
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                _movementSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, target.position) <= _arrivalThreshold)
            {
                _currentMovementTargetIndex += _movementDirection;

                if (_currentMovementTargetIndex >= _movementPoints.Count)
                {
                    _movementDirection = -1;
                    _currentMovementTargetIndex = _movementPoints.Count - 2;
                }
                else if (_currentMovementTargetIndex < 0)
                {
                    _movementDirection = 1;
                    _currentMovementTargetIndex = 1;
                }
            }

            yield return null;
        }

        _movementRoutine = null;
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
    public abstract void AttackPatternOne();
    public abstract void AttackPatternTwo();
    public abstract void AttackPatternThree();
    public abstract void RestToNeutral();
}