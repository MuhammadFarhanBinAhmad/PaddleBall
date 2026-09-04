using UnityEngine;

public class MegaExplosionAbility : ABSAbility, IExplosionContextModifier
{

    public void ModifyExplosionContextAdd(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        explosionCtx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._explosionSizeMultiplier;
        explosionCtx._Stats[STATID.MULTIPLIER_DAMAGE] = _SOAbilityEffect._baseDamageMultiplier;
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        //explosionCtx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._explosionSizeMultiplier;
        //explosionCtx._Stats[STATID.BASE_DAMAGE] = (int)(explosionCtx._Stats[STATID.BASE_DAMAGE] * explosionCtx._Stats[STATID.MULTIPLIER_DAMAGE]);
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        explosionCtx._Stats[STATID.BASE_DAMAGE] = (int)(explosionCtx._Stats[STATID.BASE_DAMAGE] * explosionCtx._Stats[STATID.MULTIPLIER_DAMAGE]);
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        throw new System.NotImplementedException(); 
    }
}
