using UnityEngine;

public class ToxicToxicToxic : ABSAbility, IToxicContextModifier
{
    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
        toxicContext._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] -= _SOAbilityEffect._timeBeforeEffectActivate;

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
