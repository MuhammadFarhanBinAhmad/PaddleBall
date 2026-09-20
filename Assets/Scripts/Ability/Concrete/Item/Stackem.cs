using UnityEngine;

public class Stackem : ABSAbility, IModifyStackableAbility
{

    public void ModifyStackableAbility(AbilityContext _abc)
    {
        _abc._Stats[STATID.MAX_STACKS] += _SOAbilityEffect._maxStacksToAdd;
    }

}
