using System.Collections;
using UnityEngine;

public class Apprentice_BurstFireAttack : BaseBossAttack
{
    [Header("Ability")]
    [SerializeField] private BrickAbilityManager _abilityManager;
    [SerializeField] private ABILITY _ability;
    [SerializeField] private SO_BrickAbilityStats _abilityStats;

    [Header("Duration")]
    [SerializeField] private float _attackDuration = 5f;

    protected override IEnumerator ExecuteAttack()
    {
        if (_abilityManager == null || _abilityStats == null)
        {
            Debug.LogError(
                $"{name}: AbilityManager or AbilityStats is NULL!"
            );

            yield break;
        }

        // Step 1: Assign the stats.
        _abilityManager.SetBrickAbilityStats(_abilityStats);

        // Step 2: Activate the ability.
        _abilityManager.ActivateAbilities(_ability);

        // Step 3: Wait for attack duration.
        yield return new WaitForSeconds(_attackDuration);

        // Step 4: Complete attack.
        CompleteAttack();
    }

    protected override void OnAttackCompleted()
    {
        _abilityManager?.ResetAllAbility();
    }

    protected override void OnAttackCancelled()
    {
        _abilityManager?.ResetAllAbility();
    }
}
