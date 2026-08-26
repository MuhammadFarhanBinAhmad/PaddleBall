using UnityEngine;

public class CheapShot : ABSAbility
{
    public override void OnHitMultiply(HitContext ctx)
    {
        bool hit = ctx._brick._brickHealthComponent.GetHealth() > ctx._brick._brickHealthComponent.GetStartingHealth() * .9f;
        if (hit)
        {
            float dmg = ctx._damageValue * _SOAbilityEffect._baseDamageMultiplier;
            ctx._damageValue = (int)dmg;
        }

    }
}
