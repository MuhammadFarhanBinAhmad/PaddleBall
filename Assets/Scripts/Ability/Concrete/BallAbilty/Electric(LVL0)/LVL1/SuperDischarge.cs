using UnityEngine;

public class SuperDischarge : ABSAbility, IDischargeContextModifier
{
    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDischargeContextAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] = _SOAbilityEffect._damagePerStack;
        dischargeCtx._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        dischargeCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;

    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        throw new System.NotImplementedException();
    }
}
