using UnityEngine;

public class DeadEyeAbility : ABSAbility, ICriticalContextModifier
{
    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
        if(critContext._Statsbool[STATID.PIERCE_BRICK])
        {
            critContext._Stats[STATID.LAYER_DESTROY] = _SOAbilityEffect._layerToDestroy;
            hitCtx._health.OnInstantKill(_SOAbilityEffect._instantKillThreshold);
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
