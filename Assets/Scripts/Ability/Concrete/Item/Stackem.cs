using UnityEngine;

public class Stackem : ABSAbility, IDischargeContextModifier, IToxicContextModifier
{
    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyDischargeContextAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
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
