using UnityEngine;

public class DoubleTrouble : ABSAbility, IToxicContextModifier
{
    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.STACKS_TO_ADD] += _SOAbilityEffect._stacksToAdd;

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
