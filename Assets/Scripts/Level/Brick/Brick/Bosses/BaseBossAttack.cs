
using System.Collections;
using UnityEngine;


public enum MOVEMENT_TYPE
{
    SPLINE_MOVEMENT,
    TELEPORT,
    STATIC
}


public abstract class BaseBossAttack : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] bool _isEnabled = true;
    [SerializeField] MOVEMENT_TYPE _movementType;
    [Header("Ability")]
    [SerializeField] internal BrickAbilityManager _abilityManager;
    [SerializeField] internal ABILITY _ability;
    [SerializeField] internal SO_BrickAbilityStats _abilityStats;
    [Header("TIME")]
    [SerializeField] internal float _attackDuration;
    [Header("Teleport")]
    public int _teleportAmount;
    public float _stayDuration;
    [Header("StaticDuration")]
    public float _staticDuration;

    protected BaseBossAttackManager _bossManager;

    private Coroutine _attackRoutine;

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
        yield return ExecuteAttack();

        if (IsRunning)
            CompleteAttack();

        _attackRoutine = null;
    }

    // Each attack defines its own behaviour and
    // decides when its execution finishes.
    protected abstract IEnumerator ExecuteAttack();

    public void StartMovementType()
    {
        switch (_movementType)
        {
            case MOVEMENT_TYPE.SPLINE_MOVEMENT:
                {
                    _bossManager.StartRandomSplineMovement();
                    break;
                }
            case MOVEMENT_TYPE.TELEPORT:
                {
                    _bossManager.SetTeleValue(_teleportAmount, _stayDuration);
                    _bossManager.StartRandomTeleportMovement();
                    break;
                }
            case MOVEMENT_TYPE.STATIC:
                {
                    _bossManager.SetStaticDuration(_staticDuration);
                    _bossManager.StartStaticPosition();
                    break;
                }
        }
    }

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

    protected bool IsMovementReady()
    {
        if (_bossManager == null)
            return false;

        switch (_movementType)
        {
            case MOVEMENT_TYPE.SPLINE_MOVEMENT:
                return _bossManager.IsTraversingSpline;

            case MOVEMENT_TYPE.TELEPORT:
                return _bossManager.IsTeleportReady;

            case MOVEMENT_TYPE.STATIC:
                return _bossManager.IsAtStaticPosition;

            default:
                return false;
        }
    }

    protected IEnumerator StartMovementAndWaitForReady()
    {
        // Step 1: Start selected movement type.
        StartMovementType();

        // Step 2: Wait for the boss to finish teleporting
        // and enter the selected movement state.
        while (IsRunning &&
               _bossManager.IsMoving &&
               !IsMovementReady())
        {
            yield return null;
        }
    }
}
