using NUnit.Framework.Internal;
using UnityEngine;

public class ThunderDomeAbility : ABSAbility, IExplosionContextModifier
{
    public SOAbilityEffect _effect;
    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
        bool isConductive = RNGService.RollCrit(_SOAbilityEffect._baseChance, _SOAbilityEffect._bonusPerFail);
        if (isConductive)
        {
            var statusCtx = new AbilityContext
            {
                _abililty = this,
                _statusType = _effect._statusType,
            };
            float dmg = hitCtx._damageValue * _SOAbilityEffect._damagePerStackMultiplier;

            statusCtx._Stats[STATID.STACKS_TO_ADD] = _effect._stacksToAdd;
            statusCtx._Stats[STATID.MAX_STACKS] = _effect._maxStacks;
            statusCtx._Stats[STATID.DAMAGE_PER_STACK] = dmg;
            statusCtx._Stats[STATID.STACK_LIFETIME] = _effect._stackLifeTime;
            statusCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _effect._timeBeforeEffectActivate;
            statusCtx._Statsbool[STATID.RESET_STACK_TIMER] = _effect._resetStackTimer;
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
