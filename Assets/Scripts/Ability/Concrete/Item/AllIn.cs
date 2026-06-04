using UnityEngine;

public class AllIn : ABSAbility, ICriticalContextModifier
{
    public void ModifyCriticalContext(HitContext hitCtx, AbilityContext critContext)
    {
        critContext._Stats[STATID.CRIT_MULTIPLIER] += _SOAbilityEffect._modiftCritMultiplier;
    }
    public override void ModifyHit(HitContext ctx)
    {
        if (!ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            float damage = ctx._damageValue;
            ctx._damageValue *= (int)(damage * _SOAbilityEffect._baseDamageMultiplier);
        }
    }
}
