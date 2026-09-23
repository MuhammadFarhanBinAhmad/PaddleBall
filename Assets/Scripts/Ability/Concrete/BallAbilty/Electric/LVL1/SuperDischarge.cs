using UnityEngine;

public class SuperDischarge : ABSAbility, IModifyStackableAbility
{


    public void ModifyStackableAbility(AbilityContext _abc)
    {
        float addOnDmg = _abc._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._damagePerStackMultiplier;

        _abc._Stats[STATID.DAMAGE_PER_STACK] += addOnDmg;
        _abc._Stats[STATID.STACK_LIFETIME] = _SOAbilityEffect._stackLifeTime;
        _abc._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] = _SOAbilityEffect._timeBeforeEffectActivate;
    }
}
