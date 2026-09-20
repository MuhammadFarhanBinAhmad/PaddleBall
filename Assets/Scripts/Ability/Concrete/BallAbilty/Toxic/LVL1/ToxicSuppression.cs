using UnityEngine;

public class ToxicSuppression: ABSAbility, IModifyStackableAbility
{
    public void ModifyStackableAbility(AbilityContext _abc)
    {
        _abc._Statsbool[STATID.AFFECTS_SPEED] = _SOAbilityEffect._affectSpeed;
        _abc._Stats[STATID.SPEED_MULTIPLIER] = _SOAbilityEffect._speedMultiplier;
    }
}
