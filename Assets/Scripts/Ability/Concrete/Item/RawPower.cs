using UnityEngine;

public class RawPower : ABSAbility, IDischargeContextModifier, IToxicContextModifier, IExplosionContextModifier, ICriticalContextModifier
{
    public override void OnHitAdd(HitContext ctx)
    {
        ctx._damageValue += _SOAbilityEffect._baseDamagePlus;
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] -= (toxicContext._Stats[STATID.DAMAGE_PER_STACK]/2);
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        explosionCtx._Stats[STATID.BASE_DAMAGE] -= (explosionCtx._Stats[STATID.BASE_DAMAGE]/2);
    }

    public void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext)
    {
        critContext._Stats[STATID.CRIT_MULTIPLIER] -= (critContext._Stats[STATID.CRIT_MULTIPLIER] / 2);
    }

    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] -= (dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] / 2);

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

    public void ModifyExplosionContextAdd(HitContext hitCtx, ExplosionContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, ExplosionContext explosionCtx)
    {
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, ExplosionContext explosionCtx)
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
