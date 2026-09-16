using UnityEngine;

public class PiercerAbility : ABSAbility, ICriticalContextModifier
{
    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
        //Increase base damage
        var critChance = _SOAbilityEffect._baseChance + hitCtx._modifyChance;
        bool isCrit = RNGService.RollCrit(critChance, _SOAbilityEffect._bonusPerFail);
        if (isCrit)
        {
            critContext._Statsbool[STATID.PIERCE_BRICK] = true;
            critContext._Stats[STATID.LAYER_DESTROY] = _SOAbilityEffect._layerToDestroy;
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
