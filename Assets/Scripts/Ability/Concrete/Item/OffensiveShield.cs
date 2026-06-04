using UnityEngine;

public class OffensiveShield : ABSAbility
{
    DeadZone _deadZone;

    private void Start()
    {
        _deadZone = FindAnyObjectByType<DeadZone>();
        _deadZone.MultipleMinusShieldValue(_SOAbilityEffect._shieldMultiplier);
        _deadZone.ResetShield();
        
    }
    public override void ModifyHit(HitContext ctx)
    {
        float dmg = (float)ctx._damageValue;
        ctx._damageValue *= Mathf.CeilToInt(dmg * _SOAbilityEffect._baseDamageMultiplier);
        print(ctx._damageValue);
    }
}
