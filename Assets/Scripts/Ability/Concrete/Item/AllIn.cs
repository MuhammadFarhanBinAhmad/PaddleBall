using UnityEngine;

public class AllIn : ABSAbility
{

    public override void OnHitMultiply(HitContext ctx)
    {
        if (ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            float damage = ctx._damageValue;
            ctx._damageValue = (int)(damage * _SOAbilityEffect._critMultiplier);
        }
    }
    public override void OnHitDivide(HitContext ctx)
    {
        if (!ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            float damage = ctx._damageValue;
            ctx._damageValue = (int)(damage * _SOAbilityEffect._baseDamageMultiplier);
        }
    }
}
