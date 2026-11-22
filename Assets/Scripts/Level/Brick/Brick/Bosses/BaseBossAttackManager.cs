
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
    float _restDuration;
    internal float _delayBetweenAttacks;

    [Header("Stun")]
    [SerializeField]  float _defaultStunDuration;

    [Header("Movement")]
    [SerializeField] internal float _movementSpeed;
    [SerializeField] internal List<SplineContainer> _movementPos = new List<SplineContainer>();
    [SerializeField] internal SO_BrickSpecialEffect _lerpInEffect, _lerpOutEffect;
    [SerializeField] internal Transform _bossTransform;
    float _teleportDelay;
    [SerializeField] float _startMovementDelay;
    [SerializeField] float _endRestDelay;

    [Header("Static")]
    [SerializeField] Transform _staticPos;
    [SerializeField] float _staticDuration;
    private bool _isAtStaticPosition;
    public bool IsAtStaticPosition => _isAtStaticPosition;

    [Header("Teleport")]
    [SerializeField, Min(0)] private int _teleportAmount = 3;
    [SerializeField] private List<Transform> _teleportPosition = new List<Transform>();
    [SerializeField, Min(0f)]
    private float _teleportStayDuration;
    [SerializeField, Min(0f)]
    private float _minTeleportDistance;
    private bool _isTeleportReady;
    public bool IsTeleportReady => _isTeleportReady;

    Coroutine _movementRoutine;
    bool _isMoving;
    private bool _isTraversingSpline;
    public bool IsTraversingSpline => _isTraversingSpline;

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
        _teleportDelay = _lerpInEffect._lerpDuration + _lerpOutEffect._lerpDuration;
        _restDuration = _teleportDelay + _startMovementDelay;
        _delayBetweenAttacks = _restDuration;
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

                yield return new WaitForSeconds(_delayBetweenAttacks);

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
        yield return new WaitForSeconds(_delayBetweenAttacks);
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
    //-----SPLINE MOVEMENT-----//
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

        if (splineLength <= 0.001f || _movementSpeed <= 0f)
        {
            FinishMovement();
            yield break;
        }

        float t = 0f;

        // Teleportation is completed.
        // Actual movement is now starting.
        _isTraversingSpline = true;

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
        _isTraversingSpline = false;

        yield return new WaitForSeconds(_endRestDelay);

        FinishMovement();
    }

    private void FinishMovement()
    {
        _isMoving = false;
        _isTraversingSpline = false;
        _isAtStaticPosition = false;
        _isTeleportReady = false;

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
        _isTraversingSpline = false;
        _isAtStaticPosition = false;
        _isTeleportReady = false;
    }
    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }

    //-----STATIC-----//
    public void SetStaticDuration(float statDuration)
    {
        _staticDuration = statDuration;
    }
    public void StartStaticPosition()
    {
        if (_stopAttacking || _isStunned)
            return;

        if (_bossTransform == null || _staticPos == null)
        {
            Debug.LogWarning(
                $"{name}: Boss transform or static position is missing."
            );
            return;
        }

        // Stop any existing movement.
        StopSplineMovement();

        _isMoving = true;
        _movementRoutine = StartCoroutine(
            StaticPositionRoutine()
        );
    }

    private IEnumerator StaticPositionRoutine()
    {
        // Step 1: Disappear.
        if (_lerpInEffect != null)
        {
            GlobalEffects.Instance.PlayLerpObject(
                _bossTransform.gameObject,
                _lerpInEffect
            );

            yield return new WaitForSeconds(
                _lerpInEffect._lerpDuration
            );
        }

        if (_stopAttacking || _isStunned)
        {
            FinishMovement();
            yield break;
        }

        // Step 2: Teleport to the static position.
        _bossTransform.position = _staticPos.position;

        // Step 3: Reappear.
        if (_lerpOutEffect != null)
        {
            GlobalEffects.Instance.PlayLerpObject(
                _bossTransform.gameObject,
                _lerpOutEffect
            );

            yield return new WaitForSeconds(
                _lerpOutEffect._lerpDuration
            );
        }

        if (_stopAttacking || _isStunned)
        {
            FinishMovement();
            yield break;
        }

        // Step 4: Boss is now at the static position.
        _isAtStaticPosition = true;

        // Remain here for the configured duration.
        yield return new WaitForSeconds(
            Mathf.Max(0f, _staticDuration)
        );

        // Step 5: Finish.
        FinishMovement();
    }
    //-----TELEPORT-----//
    public void SetTeleValue(int _teleAmt, float _stayDur)
    {
        _teleportAmount = _teleAmt;
        _teleportStayDuration = _stayDur;
    }
    public void StartRandomTeleportMovement()
    {
        if (_stopAttacking || _isStunned)
            return;

        StopSplineMovement();

        if (_bossTransform == null ||
            _teleportPosition == null ||
            _teleportPosition.Count == 0 ||
            _teleportAmount <= 0)
        {
            Debug.LogWarning(
                $"{name}: Missing teleport references or invalid teleport amount."
            );
            return;
        }

        _isMoving = true;

        _movementRoutine = StartCoroutine(
            RandomTeleportMovementRoutine()
        );
    }

    private IEnumerator RandomTeleportMovementRoutine()
    {
        // Optional initial delay before teleporting.
        if (_startMovementDelay > 0f)
            yield return new WaitForSeconds(_startMovementDelay);

        // Perform the specified number of teleports.
        for (int i = 0; i < _teleportAmount; i++)
        {
            if (_stopAttacking || _isStunned)
            {
                FinishMovement();
                yield break;
            }

            // Choose a position different from the current one.
            Transform selectedPosition = GetRandomTeleportPosition();

            if (selectedPosition == null)
            {
                Debug.LogWarning(
                    $"{name}: No valid alternative teleport position."
                );

                FinishMovement();
                yield break;
            }

            // Step 1: Disappear.
            if (_lerpInEffect != null)
            {
                GlobalEffects.Instance.PlayLerpObject(
                    _bossTransform.gameObject,
                    _lerpInEffect
                );

                yield return new WaitForSeconds(
                    _lerpInEffect._lerpDuration
                );
            }

            if (_stopAttacking || _isStunned)
            {
                FinishMovement();
                yield break;
            }

            // Step 2: Teleport to the selected position.
            _bossTransform.position = selectedPosition.position;

            // Step 3: Reappear.
            if (_lerpOutEffect != null)
            {
                GlobalEffects.Instance.PlayLerpObject(
                    _bossTransform.gameObject,
                    _lerpOutEffect
                );

                yield return new WaitForSeconds(
                    _lerpOutEffect._lerpDuration
                );
            }

            if (_stopAttacking || _isStunned)
            {
                FinishMovement();
                yield break;
            }

            // Step 4: Boss has completed at least one teleport.
            // Allow the active attack to begin.
            _isTeleportReady = true;

            // Step 5: Stay at the position before teleporting again.
            // Small minimum delay ensures the attack coroutine
            // can observe the ready state.
            yield return new WaitForSeconds(
                Mathf.Max(0.05f, _teleportStayDuration)
            );
        }

        // All teleports completed.
        FinishMovement();
    }
    private Transform GetRandomTeleportPosition()
    {
        Transform selected = null;
        int validCount = 0;

        Vector2 currentPosition = _bossTransform.position;

        float minDistanceSqr =
            _minTeleportDistance * _minTeleportDistance;

        foreach (Transform position in _teleportPosition)
        {
            if (position == null)
                continue;

            Vector2 candidatePosition = position.position;

            // Skip positions matching or too close
            // to the boss's current location.
            if ((candidatePosition - currentPosition).sqrMagnitude
                <= minDistanceSqr)
            {
                continue;
            }

            // Uniform random selection without
            // allocating a temporary list.
            validCount++;

            if (Random.Range(0, validCount) == 0)
                selected = position;
        }

        return selected;
    }
    #endregion
}
