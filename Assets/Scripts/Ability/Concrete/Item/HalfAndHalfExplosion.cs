using UnityEngine;

public class HalfAndHalfExplosion : ABSAbility, IExplosionContextModifier
{
    public void ModifyExplosionContext(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        explosionCtx._Stats[STATID.SCALE_MULTIPLIER] *= _SOAbilityEffect._explosionSizeMultiplier ;
        explosionCtx._Stats[STATID.BASE_DAMAGE] *= _SOAbilityEffect._explosionDamageMultiplier ;
    }
}
