using UnityEngine;

public class HalfAndHalfExplosion : ABSAbility, IExplosionContextModifier
{
    public void ModifyExplosionContext(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        explosionCtx._Stats[STATID.SCALE_MULTIPLIER] *= _SOAbilityEffect._explosionSizeMultiplier ;
        explosionCtx._Stats[STATID.BASE_DAMAGE] *= _SOAbilityEffect._explosionDamageMultiplier ;
    }

    public void ModifyExplosionContextAdd(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        throw new System.NotImplementedException();
    }
}
