using UnityEngine;

public class PatientIsKey : ABSAbility, IToxicContextModifier, IDischargeContextModifier
{
    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
        dischargeCtx._Stats[STATID.STACK_LIFETIME] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] *= _SOAbilityEffect._baseDamageMultiplier;
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
        toxicContext._Stats[STATID.STACK_LIFETIME] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;

    }

    public void ModifyToxicContextDivide(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextMultiple(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] *= _SOAbilityEffect._baseDamageMultiplier;
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
    }
}
