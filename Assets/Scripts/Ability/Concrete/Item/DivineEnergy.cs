using UnityEngine;

public class DivineEnergy : ABSAbility
{
    AbilityManager _manager;

    private void Awake()
    {
        _manager = FindAnyObjectByType<AbilityManager>();
    }

    public override void OnHitAdd(HitContext ctx)
    {
        float dmg = _manager.GetTotalSpell() * (_SOAbilityEffect._baseDamageMultiplier * ctx._damageValue);
        ctx._damageValue += (int)dmg;
    }
}
