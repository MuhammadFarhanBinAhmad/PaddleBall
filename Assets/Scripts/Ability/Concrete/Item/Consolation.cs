using UnityEngine;

public class Consolation : ABSAbility
{

    public override void OnHitAdd(HitContext ctx)
    {
        if(!ctx._status.HasFlag(STATUSTYPE.CRIT))
        {
            ctx._damageValue += _SOAbilityEffect._baseDamagePlus;
        }
    }
}
