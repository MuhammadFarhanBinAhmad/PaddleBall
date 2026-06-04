using UnityEngine;

public class PatientIsKey : ABSAbility, IToxicContextModifier, IDischargeContextModifier
{
    public void ModifyDischargeContext(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] *= _SOAbilityEffect._baseDamageMultiplier;
        dischargeCtx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
        dischargeCtx._Stats[STATID.STACK_LIFETIME] +=  _SOAbilityEffect._modifyTimeBeforeEffectActivate;
    }

    public void ModifyToxicContext(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] *= _SOAbilityEffect._baseDamageMultiplier;
        toxicContext._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
    }
}
