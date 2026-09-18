using UnityEngine;

public class RemnantsAbility : ABSAbility, IExplosionContextModifier
{
    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
        bool isRemnants = RNGService.RollCrit(_SOAbilityEffect._baseChance + hitCtx._modifyChance, _SOAbilityEffect._bonusPerFail);
        if (isRemnants)
        {
            explosionCtx._statusType = _SOAbilityEffect._statusType;
        }
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }
}
