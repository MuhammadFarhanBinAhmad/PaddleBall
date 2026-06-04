using UnityEngine;

public class RawPower : ABSAbility, IDischargeContextModifier, IToxicContextModifier
{
    public override void ModifyHit(HitContext ctx)
    {
        ctx._damageValue += _SOAbilityEffect._baseDamagePlus;
    }

    public void ModifyDischargeContext(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] -= _SOAbilityEffect._baseDamageMinus;
    }

    public void ModifyToxicContext(AbilityContext toxicContext)
    {
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] -= _SOAbilityEffect._baseDamageMinus;
    }

}
