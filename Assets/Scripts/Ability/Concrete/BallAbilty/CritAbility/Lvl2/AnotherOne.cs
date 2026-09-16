using UnityEngine;

public class AnotherOne : ABSAbility, ICriticalContextModifier
{
    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
        if (critContext._Statsbool[STATID.PIERCE_BRICK])
        {
            critContext._Stats[STATID.LAYER_DESTROY] += _SOAbilityEffect._layerToDestroy;
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
