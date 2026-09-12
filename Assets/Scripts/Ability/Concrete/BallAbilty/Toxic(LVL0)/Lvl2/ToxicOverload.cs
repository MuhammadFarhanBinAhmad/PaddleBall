using UnityEditor;
using UnityEngine;

public class ToxicOverload : ABSAbility
{
    public override void OnHitResolved(HitContext ctx)
    {
        print("hit");
        StatusInstance si = ctx._health.GetStatusInstance(STATUSTYPE.TOXIC);
        if (si.stacks == 2)
        {
            si.stacks = 0;
            ctx._health.RemoveStatusVFX(STATUSTYPE.TOXIC);
            float dmg = si.maxStacks * (si.damagePerStack * _SOAbilityEffect._baseDamageMultiplier);
            ctx._health.OnDamage((int)dmg);
            print("Explode");
        }
    }
}
