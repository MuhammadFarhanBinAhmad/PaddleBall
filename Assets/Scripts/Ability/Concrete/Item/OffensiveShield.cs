using UnityEngine;

public class OffensiveShield : ABSAbility
{

    private void Start()
    {
        DeadZone _deadZone;
        _deadZone = FindAnyObjectByType<DeadZone>();
        _deadZone.MultipleMinusShieldValue(_SOAbilityEffect._shieldMultiplier);
        _deadZone.ResetShield();
        FindAnyObjectByType<ShieldUIManager>().UpdateShieldUI();
    }
    public override void OnHitMultiply (HitContext ctx)
    {
        float dmg = (float)ctx._damageValue;
        ctx._damageValue = Mathf.CeilToInt(dmg * _SOAbilityEffect._baseDamageMultiplier);
    }
}
