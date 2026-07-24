using System.Collections;
using UnityEngine;

public class boss_TheApprenticeAttackManager : BaseBossAttackManager
{
    public Transform _target;
    public Transform _spawnPos;
    [Header("Attack One")]
    [SerializeField] GameObject _projectile;
    public float _attackOneAttackDuration;
    public float _attackOneShotInterval;
    [Header("Attack Two")]
    [SerializeField] GameObject _magicProjectile;
    public float _attackTwoAttackDuration;


    private void Awake()
    {
        _target = FindAnyObjectByType<PaddleBallShooter>().transform;
    }
    public override void AttackPatternOne()
    {
        Debug.Log("Attack One: Charge Shot");
        BeginTimedAttack(0);
        StartCoroutine(ShootMagicProjectile());
    }

    public override void AttackPatternTwo()
    {
        //Debug.Log("Attack One: projectile count based");
        //BeginHitCountAttack(3);
        //// Spawn your projectiles here
        //// Each projectile hit should call RegisterAttackHit()
        Debug.Log("Attack Two: continuous point shooting");
        BeginPointAttack();
        StartCoroutine(ShootNormalProjectile());
    }

    public override void AttackPatternThree()
    {
        Debug.Log("Attack Three: None");
        //BeginManualAttack();
        //// Call CompleteCurrentAttack() when your custom condition is met
        CompleteCurrentAttack();
    }

    public override void RestToNeutral()
    {
        Debug.Log("Boss is resting / resetting to neutral");
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
    IEnumerator ShootNormalProjectile()
    {
        while (IsAttackActive)
        {
            GameObject proj = Instantiate(_projectile, _spawnPos.position, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            ep.ShootProjectile(_target);
            yield return new WaitForSeconds(_attackOneShotInterval);
        }
    }
    IEnumerator ShootMagicProjectile()
    {
        while (IsAttackActive)
        {
            print("charging shot");
            yield return new WaitForSeconds(_attackOneAttackDuration);
            GameObject proj = Instantiate(_magicProjectile, _spawnPos.position, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            ep.ShootProjectile(_target);
            print("Rest");
            yield return new WaitForSeconds(_attackOneAttackDuration);
            print("Complete Rest");
        }
    }

}
