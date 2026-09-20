using UnityEngine;

public class DoubleTrouble : ABSAbility, IModifyStackableAbility
{
    public void ModifyStackableAbility(AbilityContext _abc)
    {
        _abc._Stats[STATID.STACKS_TO_ADD] += _SOAbilityEffect._stacksToAdd;
    }

}
