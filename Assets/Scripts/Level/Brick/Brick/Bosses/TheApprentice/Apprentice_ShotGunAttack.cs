
using System.Collections;
using UnityEngine;

public class Apprentice_ShotGunAttack : BaseBossAttack
{


    protected override IEnumerator ExecuteAttack()
    {
        // Step 1: Assign the stats.
        _abilityManager.SetBrickAbilityStats(_abilityStats);

        // Step 2: Activate the ability.
        _abilityManager.ActivateAbilities(_ability);

        // Step 3: Execute based on attack type.

        _bossManager.StartRandomSplineMovement();
        // Wait until movement finishes.
        while (_bossManager.IsMoving)
        {
            yield return null;
        }
        //switch (_executionType)
        //{
        //    case BOSS_ATTACK_EXECUTION_TYPE.TIME:
        //        {
        //            yield return new WaitForSeconds(_attackDuration);
        //            break;
        //        }

        //    case BOSS_ATTACK_EXECUTION_TYPE.MOVING:
        //        {
        //            // Start movement along a random spline.


        //            break;
        //        }

        //    case BOSS_ATTACK_EXECUTION_TYPE.NONE:
        //        {
        //            break;
        //        }
        //}

        // Step 4: Complete attack.
        CompleteAttack();
    }

    protected override void OnAttackCompleted()
    {
        _abilityManager?.ResetAllAbility();
    }

    protected override void OnAttackCancelled()
    {
        _bossManager.StopSplineMovement();
        _abilityManager?.ResetAllAbility();
    }
}
