using UnityEngine;

public class StunnerAbility : ABSAbility , IExplosionContextModifier
{
    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
        bool isStunned = RNGService.RollCrit(_SOAbilityEffect._baseChance, _SOAbilityEffect._bonusPerFail);

        if (isStunned)
        {
            var statusCtx = new AbilityContext
            {
                _abililty = this,
                _statusType = _SOAbilityEffect._statusType,
            };

            statusCtx._Stats[STATID.STACKS_TO_ADD] = _SOAbilityEffect._stacksToAdd;
            statusCtx._Stats[STATID.MAX_STACKS] = _SOAbilityEffect._maxStacks;
            statusCtx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
            statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
            statusCtx._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
            statusCtx._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
            explosionCtx.SetContext(statusCtx);
        }


    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }
}
