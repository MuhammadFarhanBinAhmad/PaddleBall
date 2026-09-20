using UnityEngine;

public class PatientIsKey : ABSAbility, IModifyStackableAbility
{
    public void ModifyStackableAbility(AbilityContext _abc)
    {
        _abc._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
        _abc._Stats[STATID.STACK_LIFETIME] += _SOAbilityEffect._modifyTimeBeforeEffectActivate;
        _abc._Stats[STATID.DAMAGE_PER_STACK] *= _SOAbilityEffect._baseDamageMultiplier;

    }
}
