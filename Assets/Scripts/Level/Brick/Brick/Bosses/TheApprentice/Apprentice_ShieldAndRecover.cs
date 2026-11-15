
using System.Collections;
using UnityEngine;

public class Apprentice_ShieldAndRecover : BaseBossAttack
{
    [Header("References")]
    [SerializeField] private BaseBossBrick _baseBossBrick;

    [Header("Health Recovery")]
    [SerializeField] private int _healAmount;
    [SerializeField] private float _healInterval;
    [SerializeField] private GameObject _healEffect;

    private Coroutine _healRoutine;

    protected override IEnumerator ExecuteAttack()
    {
        // Step 1: Start movement sequence.
        _bossManager.StartRandomSplineMovement();

        // Step 2: Wait until teleportation finishes
        // and actual spline movement begins.
        while (_bossManager.IsMoving &&
               !_bossManager.IsTraversingSpline)
        {
            yield return null;
        }

        // If movement failed or was interrupted,
        // do not activate the attack.
        if (!_bossManager.IsTraversingSpline)
            yield break;

        // Step 3: Activate defensive ability.
        _abilityManager.SetBrickAbilityStats(_abilityStats);
        _abilityManager.ActivateAbilities(_ability);

        // Step 4: Start health regeneration.
        _healRoutine = StartCoroutine(RecoverHealthRoutine());

        // Step 5: Wait until movement finishes.
        while (_bossManager.IsMoving)
        {
            yield return null;
        }

        // Step 6: Finish attack.
        CompleteAttack();
    }

    private IEnumerator RecoverHealthRoutine()
    {
        if (_healEffect != null)
            _healEffect.SetActive(true);

        while (IsRunning)
        {
            yield return new WaitForSeconds(
                Mathf.Max(0.01f, _healInterval)
            );

            if (!IsRunning)
                yield break;

            if (_baseBossBrick != null)
                _baseBossBrick.RecoverHealth(_healAmount);
        }
    }

    private void StopHealing()
    {
        if (_healRoutine != null)
        {
            StopCoroutine(_healRoutine);
            _healRoutine = null;
        }
    }

    private void CleanupAttack()
    {
        StopHealing();

        if (_healEffect != null)
            _healEffect.SetActive(false);

        _bossManager?.StopSplineMovement();
        _abilityManager?.ResetAllAbility();
    }

    protected override void OnAttackCompleted()
    {
        CleanupAttack();
    }

    protected override void OnAttackCancelled()
    {
        CleanupAttack();
    }
}
