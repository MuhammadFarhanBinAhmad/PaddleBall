using UnityEngine;

public class ControlledFirePower : ABSAbility, IExplosionContextModifier
{
    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
        explosionCtx._Stats[STATID.SCALE_MULTIPLIER] *= _SOAbilityEffect._explosionSizeMultiplier;
        explosionCtx._Stats[STATID.BASE_DAMAGE] *= _SOAbilityEffect._explosionDamageMultiplier;
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }
}
