using UnityEngine;

public class ShellShockAbility : ABSAbility
{
    public override void OnHitResolved(HitContext ctx)
    {
        bool isCrit = RNGService.RollCrit(_SOAbilityEffect._baseChance, _SOAbilityEffect._bonusPerFail);
        if (isCrit)
        {
            if (!ctx._health.HasStatus(STATUSTYPE.CHARGE))
            {
                ctx._health.AddStatus(STATUSTYPE.CHARGE);
                ctx._health.ModifyDamageMultiplier(_SOAbilityEffect._baseDamageMultiplier);
                return;
            }
        }
    }
}
