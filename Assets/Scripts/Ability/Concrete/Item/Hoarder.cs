using UnityEngine;

public class Hoarder : ABSAbility
{
    TowerManager _towerManager;

    private void Awake()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
    }
    public override void OnHitAdd(HitContext ctx)
    {
        ctx._damageValue += _towerManager.GetTotalPureEssenceCount() * _SOAbilityEffect._baseDamagePlus;
    }
}
