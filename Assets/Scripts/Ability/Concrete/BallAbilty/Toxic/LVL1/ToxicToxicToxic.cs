using UnityEngine;

public class ToxicToxicToxic : ABSAbility, IModifyStackableAbility
{
    public void ModifyStackableAbility(AbilityContext _abc)
    {
        _abc._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
        _abc._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] -= _SOAbilityEffect._timeBeforeEffectActivate;
    }
}
