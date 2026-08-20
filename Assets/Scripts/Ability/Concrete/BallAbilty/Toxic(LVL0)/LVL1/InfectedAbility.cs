using UnityEngine;

public class InfectedAbility : ABSAbility, IToxicContextModifier
{
    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        toxicContext._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
        toxicContext._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
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
