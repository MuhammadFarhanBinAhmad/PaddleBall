using UnityEngine;

public class Stackem : ABSAbility, IDischargeContextModifier, IToxicContextModifier, ICriticalContextModifier
{
    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextDivide(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextMultiply(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeContextAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
    }

    public void ModifyToxicContextDivide(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextMultiple(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
    }
}
