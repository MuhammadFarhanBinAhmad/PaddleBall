using UnityEngine;

public class AllIn : ABSAbility
{

    public override void OnHitResolved(HitContext ctx)
    {
        if (ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            float damage = ctx._damageValue * _SOAbilityEffect._critMultiplier;
            ctx._damageValue = (int)damage;
        }
        if (!ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            float damage = ctx._damageValue;
            ctx._damageValue = (int)(damage * _SOAbilityEffect._baseDamageMultiplier);
        }
    }
}
