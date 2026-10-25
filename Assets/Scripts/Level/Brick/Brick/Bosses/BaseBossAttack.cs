
using System.Collections;
using UnityEngine;

enum BOSS_ATTACK_EXECUTION_TYPE
{
    NONE,
    TELEPORT,
    MOVING,
    TIME
}


public abstract class BaseBossAttack : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] string _attackName;
    [SerializeField] bool _isEnabled = true;
    [SerializeField] internal BOSS_ATTACK_EXECUTION_TYPE _executionType;
    [Header("Ability")]
    [SerializeField] internal BrickAbilityManager _abilityManager;
    [SerializeField] internal ABILITY _ability;
    [SerializeField] internal SO_BrickAbilityStats _abilityStats;

    [Header("TIME")]
    [SerializeField] internal float _attackDuration;
    [Header("Teleport")]
    [SerializeField] internal int _teleportAmount;

    protected BaseBossAttackManager _bossManager;

    private Coroutine _attackRoutine;

    public string AttackName => _attackName;
    public bool IsAvailable => _isEnabled && isActiveAndEnabled;
    public bool IsRunning { get; private set; }
    public bool IsCompleted { get; private set; }

    // Called once by the boss manager.
    public virtual void Initialize(BaseBossAttackManager manager)
    {
        _bossManager = manager;
    }

    public void StartAttack()
    {
        if (IsRunning || !IsAvailable)
            return;

        IsCompleted = false;
        IsRunning = true;

        _attackRoutine = StartCoroutine(AttackWrapper());
    }

    private IEnumerator AttackWrapper()
    {
        // Execute the unique attack behaviour.
        yield return ExecuteAttack();

        // Don't complete an attack that was cancelled.
        if (IsRunning)
            CompleteAttack();
    }

    // Each attack defines its own behaviour and
    // decides when its execution finishes.
    protected abstract IEnumerator ExecuteAttack();

    protected void CompleteAttack()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        IsCompleted = true;

        OnAttackCompleted();
    }

    public void CancelAttack()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        IsCompleted = false;

        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }

        OnAttackCancelled();
    }

    protected virtual void OnAttackCompleted() { }

    protected virtual void OnAttackCancelled() { }

    protected virtual void OnDisable()
    {
        CancelAttack();
    }
}
