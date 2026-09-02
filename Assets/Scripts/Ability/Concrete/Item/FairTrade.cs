using UnityEngine;

public class FairTrade : ABSAbility
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindAnyObjectByType<TowerManager>().AddEssenceCardBonus(_SOAbilityEffect._essenceValueAdd);
        DeadZone _deadZone;
        _deadZone = FindAnyObjectByType<DeadZone>();
        _deadZone.MultipleMinusShieldValue(_SOAbilityEffect._shieldMultiplier);
        _deadZone.ResetShield();
        FindAnyObjectByType<ShieldUIManager>().UpdateShieldUI();
    }
}
