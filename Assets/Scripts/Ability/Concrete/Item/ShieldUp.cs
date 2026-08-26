using UnityEngine;

public class ShieldUp : ABSAbility
{
    private void Start()
    {
        DeadZone _deadZone;
        _deadZone = FindAnyObjectByType<DeadZone>();
        _deadZone.AddShieldValue(_SOAbilityEffect._shieldAdd);
        _deadZone.ResetShield();
        FindAnyObjectByType<ShieldUIManager>().UpdateShieldUI();
    }
}
