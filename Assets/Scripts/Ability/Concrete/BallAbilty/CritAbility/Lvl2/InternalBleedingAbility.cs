using UnityEngine;

public class InternalBleedingAbility : ABSAbility, ICriticalContextModifier
{
    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
        if (hitCtx._health.HasStatus(STATUSTYPE.BLEEDING))
        {
            int stack = hitCtx._health.GetStatusStack(STATUSTYPE.BLEEDING);
            critContext._Stats[STATID.CRIT_CHANCE] += _SOAbilityEffect._baseChance * stack;
            critContext._Stats[STATID.CRIT_MULTIPLIER] += _SOAbilityEffect._critMultiplier * stack;
        }

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
}
