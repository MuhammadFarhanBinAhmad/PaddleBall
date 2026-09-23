using UnityEngine;

public class RawPower : ABSAbility, IDischargeContextModifier, IExplosionContextModifier, ICriticalContextModifier
{


    public override void BeforeHitMultiply(HitContext ctx)
    {
        ctx._damageValue += ctx._damageValue;
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] -= (toxicContext._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._baseDamageMultiplier);
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx)
    {
        explosionCtx._Stats[STATID.BASE_DAMAGE] -= (explosionCtx._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);
    }

    public void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext)
    {
        critContext._Stats[STATID.BASE_DAMAGE] -= (critContext._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);
    }

    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {

    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] -= (dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._baseDamageMultiplier);
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextMultiple(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextDivide(AbilityContext toxicContext)
    {
    }

    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextMultiply(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextDivide(HitContext hitCtx, AbilityContext critContext)
    {
    }

}
