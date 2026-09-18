using UnityEngine;

public class RemnantsAbility : ABSAbility, IExplosionContextModifier
{
    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
        bool isCrit = RNGService.RollCrit(_SOAbilityEffect._baseChance + hitCtx._modifyChance, _SOAbilityEffect._bonusPerFail);
        if (isCrit)
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
