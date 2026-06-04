using UnityEngine;

public class Stackem : ABSAbility , IDischargeContextModifier , IToxicContextModifier
{
    public void ModifyDischargeContext(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
    }

    public void ModifyToxicContext(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
    }
}
