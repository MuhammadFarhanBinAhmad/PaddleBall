using System.Collections;
using UnityEngine;

public class Apprentice_BurstFireAttack : BaseBossAttack
{

    protected override IEnumerator ExecuteAttack()
    {
        if (_abilityManager == null || _abilityStats == null)
        {
            Debug.LogError(
                $"{name}: AbilityManager or AbilityStats is NULL!"
            );

            yield break;
        }

        // Step 1: Start the selected movement type
        // and wait until the boss is ready to attack.
        yield return StartMovementAndWaitForReady();

        // Step 2: If movement failed or was interrupted,
        // do not activate the ability.
        if (!IsRunning || !IsMovementReady())
            yield break;

        // Step 3: Assign the stats and activate the ability.
        _abilityManager.SetBrickAbilityStats(_abilityStats);
        _abilityManager.ActivateAbilities(_ability);


        // Step 5: Wait for the movement or static
        // duration to finish.
        while (_bossManager.IsMoving)
        {
            yield return null;
        }

        // Step 6: Complete attack.
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
