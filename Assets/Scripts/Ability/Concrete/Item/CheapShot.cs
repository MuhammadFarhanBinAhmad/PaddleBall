using UnityEngine;

public class CheapShot : ABSAbility
{
    public override void BeforeHitMultiply(HitContext ctx)
    {
        bool hit = ctx._health.GetHealth() > ctx._health.GetStartingHealth() * .9f;
        if (hit)
        {
            float dmg = ctx._damageValue * _SOAbilityEffect._baseDamageMultiplier;
            ctx._damageValue = (int)dmg;
        }

    }
}
