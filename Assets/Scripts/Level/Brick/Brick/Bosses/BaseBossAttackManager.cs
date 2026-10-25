
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public abstract class BaseBossAttackManager : MonoBehaviour
{
    [Header("Boss Attacks")]
    [SerializeField]  List<BaseBossAttack> _attacks = new();
    [SerializeField]  bool _shuffleAttacks = true;

    [Header("Boss Attack Flow")]
    [SerializeField] protected float _restDuration;
    [SerializeField] protected float _delayBetweenAttacks;

    [Header("Stun")]
    [SerializeField]  float _defaultStunDuration;

    [Header("Movement")]
    [SerializeField] internal float _movementSpeed;
    [SerializeField] internal List<SplineContainer> _movementPos = new List<SplineContainer>();
    [SerializeField] internal SO_BrickSpecialEffect _lerpInEffect, _lerpOutEffect;
    [SerializeField] internal Transform _bossTransform;
    [SerializeField] float _teleportDelay;
    [SerializeField] float _startMovementDelay;
    [SerializeField] float _endRestDelay;
    Coroutine _movementRoutine;
    bool _isMoving;


    protected bool _stopAttacking = true;
    protected bool _isStunned;

    Coroutine _bossRoutine;
    BaseBossAttack _currentAttack;

    readonly List<BaseBossAttack> _attackOrder = new();

    public BaseBossAttack CurrentAttack => _currentAttack;
    public bool IsStunned => _isStunned;
    public bool IsMoving => _isMoving;
    protected virtual void Awake()
    {
        foreach (BaseBossAttack attack in _attacks)
        {
            if (attack != null)
                attack.Initialize(this);
        }
    }

    public virtual void StartBossFight()
    {
        if (_bossRoutine != null)
            return;

        _stopAttacking = false;
        _isStunned = false;

        _bossRoutine = StartCoroutine(BossAttackLoop());
    }

    private IEnumerator BossAttackLoop()
    {
        yield return new WaitForSeconds(_delayBetweenAttacks);
        while (!_stopAttacking)
        {
            BuildAttackList();

            if (_attackOrder.Count == 0)
            {
                Debug.LogWarning(
                    $"{name}: No boss attacks available."
                );

                _bossRoutine = null;
                yield break;
            }

            foreach (BaseBossAttack attack in _attackOrder)
            {
                if (_stopAttacking)
                    yield break;

                while (_isStunned && !_stopAttacking)
                    yield return null;

                if (_stopAttacking)
                    yield break;

                if (attack == null || !attack.IsAvailable)
                    continue;

                yield return ExecuteAttack(attack);

                if (_stopAttacking)
                    yield break;

                while (_isStunned && !_stopAttacking)
                    yield return null;

                if (_stopAttacking)
                    yield break;

                // Delay between attacks.
                if (_delayBetweenAttacks > 0f)
                    yield return new WaitForSeconds(
                        _delayBetweenAttacks
                    );
            }

            if (_stopAttacking)
                yield break;

            // Return to resting position after all attacks.
            yield return RestSequence();
        }

        _bossRoutine = null;
    }

    private void BuildAttackList()
    {
        _attackOrder.Clear();

        foreach (BaseBossAttack attack in _attacks)
        {
            if (attack != null && attack.IsAvailable)
                _attackOrder.Add(attack);
        }

        if (_shuffleAttacks)
            ShuffleList(_attackOrder);
    }

    private void ShuffleList(List<BaseBossAttack> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }

    private IEnumerator ExecuteAttack(BaseBossAttack attack)
    {
        _currentAttack = attack;

        attack.StartAttack();

        // Wait until this attack completes or is interrupted.
        while (!_stopAttacking &&
               !_isStunned &&
               attack.IsRunning)
        {
            yield return null;
        }

        if ((_stopAttacking || _isStunned) && attack.IsRunning)
            attack.CancelAttack();

        _currentAttack = null;
    }

    private IEnumerator RestSequence()
    {
        // Boss-specific resting movement.
        yield return MoveToRestPosition();

        if (_stopAttacking)
            yield break;

        while (_isStunned && !_stopAttacking)
            yield return null;

        if (_stopAttacking)
            yield break;

        yield return new WaitForSeconds(_restDuration);
    }

    // Each boss can define how it returns to rest.
    protected virtual IEnumerator MoveToRestPosition()
    {
        yield break;
    }

    public void StunBoss(float duration)
    {
        if (_stopAttacking || _isStunned)
            return;

        _isStunned = true;

        _currentAttack?.CancelAttack();
        _currentAttack = null;

        OnBossStunned();

        StartCoroutine(StunRoutine(
            Mathf.Max(0f, duration)
        ));
    }

    private IEnumerator StunRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (_stopAttacking)
            yield break;

        _isStunned = false;

        OnBossStunEnded();
    }

    protected virtual void OnBossStunned() { }
    protected virtual void OnBossStunEnded() { }

    public virtual void StopBossAttack()
    {
        _stopAttacking = true;

        _currentAttack?.CancelAttack();
        _currentAttack = null;

        StopAllCoroutines();
        _bossRoutine = null;
        _isStunned = false;
    }

    #region Movement
    public void StartRandomSplineMovement()
    {
        if (_stopAttacking || _isStunned)
            return;

        StopSplineMovement();

        if (_bossTransform == null ||
            _movementPos == null ||
            _movementPos.Count == 0)
        {
            Debug.LogWarning($"{name}: Missing boss transform or splines.");
            return;
        }

        _isMoving = true;
        _movementRoutine = StartCoroutine(RandomSplineMovementRoutine());
    }

    private IEnumerator RandomSplineMovementRoutine()
    {

        yield return new WaitForSeconds(_startMovementDelay);

        // Select a valid random spline.
        List<SplineContainer> validSplines =
            _movementPos.FindAll(s => s != null);

        if (validSplines.Count == 0)
        {
            _isMoving = false;
            _movementRoutine = null;
            yield break;
        }

        SplineContainer selectedSpline =
            validSplines[Random.Range(0, validSplines.Count)];

        // Calculate the starting position in world space.
        Vector3 startWorldPosition =
            selectedSpline.transform.TransformPoint(
                selectedSpline.EvaluatePosition(0f)
            );

        // Step 1: Disappear.
        if (_lerpInEffect != null)
        {
            GlobalEffects.Instance.PlayLerpObject(
                _bossTransform.gameObject,
                _lerpInEffect
            );
            yield return new WaitForSeconds(
                _lerpInEffect._lerpDuration + _teleportDelay
            );
        }

        if (_stopAttacking || _isStunned)
        {
            FinishMovement();
            yield break;
        }

        // Step 2: Teleport to spline start.
        _bossTransform.position = startWorldPosition;
        // Step 3: Reappear.
        if (_lerpOutEffect != null)
        {
            GlobalEffects.Instance.PlayLerpObject(
                _bossTransform.gameObject,
                _lerpOutEffect
            );
            yield return new WaitForSeconds(
                _lerpOutEffect._lerpDuration + _teleportDelay
            );
        }

        if (_stopAttacking || _isStunned)
        {
            FinishMovement();
            yield break;
        }

        // Step 4: Move along the spline.
        float splineLength = selectedSpline.CalculateLength();

        if (splineLength <= 0.001f)
        {
            FinishMovement();
            yield break;
        }

        float t = 0f;

        while (t < 1f && !_stopAttacking && !_isStunned)
        {
            t += (_movementSpeed / splineLength) * Time.deltaTime;
            t = Mathf.Clamp01(t);

            Vector3 splineWorldPosition =
                selectedSpline.transform.TransformPoint(
                    selectedSpline.EvaluatePosition(t)
                );

            _bossTransform.position = splineWorldPosition;

            yield return null;
        }

        if (!_stopAttacking && !_isStunned)
        {
            _bossTransform.position =
                selectedSpline.transform.TransformPoint(
                    selectedSpline.EvaluatePosition(1f)
                );
        }

        yield return new WaitForSeconds(_endRestDelay);

        FinishMovement();
    }

    private void FinishMovement()
    {
        _isMoving = false;
        _movementRoutine = null;
    }

    public void StopSplineMovement()
    {
        if (_movementRoutine != null)
        {
            StopCoroutine(_movementRoutine);
            _movementRoutine = null;
        }

        _isMoving = false;
    }
    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }

    #endregion
}
