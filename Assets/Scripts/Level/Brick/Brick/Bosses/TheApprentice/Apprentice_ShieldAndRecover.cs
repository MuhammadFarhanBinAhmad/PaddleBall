
using System.Collections;
using UnityEngine;

public class Apprentice_ShieldAndRecover : BaseBossAttack
{
    [Header("References")]
    [SerializeField] private BaseBossBrick _baseBossBrick;

    [Header("Health Recovery")]
    [SerializeField] private int _healAmount = 5;
    [SerializeField] private float _healInterval = 1f;
    [SerializeField] private int _maxHealTicks = 0;

    private Coroutine _healRoutine;

    protected override IEnumerator ExecuteAttack()
    {
        // Activate defensive ability.
        _abilityManager.SetBrickAbilityStats(_abilityStats);
        _abilityManager.ActivateAbilities(_ability);

        // Start health regeneration.
        _healRoutine = StartCoroutine(RecoverHealthRoutine());

        // Start movement.
        _bossManager.StartRandomSplineMovement();

        // Wait until movement is completed.
        while (_bossManager.IsMoving)
        {
            yield return null;
        }

        // Finish attack.
        CompleteAttack();
    }

    private IEnumerator RecoverHealthRoutine()
    {
        int healTicks = 0;

        while (IsRunning)
        {
            yield return new WaitForSeconds(
                Mathf.Max(0.01f, _healInterval)
            );

            if (!IsRunning)
                yield break;

            if (_baseBossBrick != null)
                _baseBossBrick.RecoverHealth(_healAmount);

            healTicks++;
        }

        _healRoutine = null;
    }

    private void StopHealing()
    {
        if (_healRoutine != null)
        {
            StopCoroutine(_healRoutine);
            _healRoutine = null;
        }
    }

    protected override void OnAttackCompleted()
    {
        StopHealing();
        _bossManager.StopSplineMovement();
        _abilityManager?.ResetAllAbility();
    }
}
