using UnityEditor;
using UnityEngine;

public class ToxicOverload : ABSAbility
{
    public override void OnHitResolved(HitContext ctx)
    {
        StatusInstance si = ctx._health.GetStatusInstance(STATUSTYPE.TOXIC);
        if (si.stacks == si.maxStacks)
        {
            si.stacks = 0;
            ctx._health.RemoveStatusVFX(STATUSTYPE.TOXIC);
            float dmg = si.maxStacks * (si.damagePerStack * _SOAbilityEffect._baseDamageMultiplier);
            ctx._health.OnDamage((int)dmg,STATUSTYPE.TOXIC);
        }
    }
}
