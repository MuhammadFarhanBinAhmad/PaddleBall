
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossAttackManager : MonoBehaviour
{
    [Header("Boss Attacks")]
    [SerializeField] private List<BaseBossAttack> _attacks = new();
    [SerializeField] private bool _shuffleAttacks = true;

    [Header("Boss Attack Flow")]
    [SerializeField] protected float _restDuration = 2f;
    [SerializeField] protected float _delayBetweenAttacks = 0.5f;

    [Header("Stun")]
    [SerializeField] private float _defaultStunDuration = 2f;

    [Header("Movement")]
    [SerializeField] protected float _movementSpeed = 5f;

    protected bool _stopAttacking = true;
    protected bool _isStunned;

    private Coroutine _bossRoutine;
    private BaseBossAttack _currentAttack;

    private readonly List<BaseBossAttack> _attackOrder = new();

    public BaseBossAttack CurrentAttack => _currentAttack;
    public bool IsStunned => _isStunned;

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

    public void SetSpeed(float speed)
    {
        _movementSpeed = speed;
    }
}
