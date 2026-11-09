using UnityEngine;

public class ToxicToxicToxic : ABSAbility, IModifyStackableAbility
{
    public void ModifyStackableAbility(AbilityContext ctx)
    {
        if (ctx._statusType != STATUSTYPE.TOXIC)
            return;

        ctx._Stats[STATID.MAX_STACKS] +=
            _SOAbilityEffect._maxStacksToAdd;

        ctx._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] -=
            _SOAbilityEffect._timeBeforeEffectActivate;
    }
}
