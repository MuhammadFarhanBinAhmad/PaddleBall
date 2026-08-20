using UnityEngine;

public class LetItRide : ABSAbility
{
    private void Start()
    {
        StoreAbilityManager sam =FindAnyObjectByType<StoreAbilityManager>();
        sam.ChangeRerollValue(_SOAbilityEffect._abilityBaseDamageValue);
        FindAnyObjectByType<StoreOverlayUI>().UpdateRerollText();

    }
}
