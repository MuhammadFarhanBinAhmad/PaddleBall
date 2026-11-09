using UnityEngine;

public class SuperDischarge : ABSAbility, IModifyStackableAbility
{


    public void ModifyStackableAbility(AbilityContext ctx)
    {
        if (ctx._statusType != STATUSTYPE.DISCHARGE)
            return;

        float additionalDamage =
            ctx._Stats[STATID.DAMAGE_PER_STACK] *
            _SOAbilityEffect._damagePerStackMultiplier;

        ctx._Stats[STATID.DAMAGE_PER_STACK] += additionalDamage;

        ctx._Stats[STATID.STACK_LIFETIME] =
            _SOAbilityEffect._stackLifeTime;

        ctx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] =
            _SOAbilityEffect._timeBeforeEffectActivate;
    }
}
