using UnityEngine;

public class EnergyTransfer : ABSAbility
{
    TimeManager _timeManager;
    TowerManager _towerManager;


    float _bonusPercentage;

    private void Awake()
    {
        _timeManager = FindAnyObjectByType<TimeManager>();
        _towerManager = FindAnyObjectByType<TowerManager>();
    }
    private void Start()
    {
        _timeManager._dayPass += ConvertEssence;
    }
    void ConvertEssence()
    {
        _bonusPercentage = _towerManager.GetTotalPureEssenceCount() * _SOAbilityEffect._baseDamageMultiplier;
        _towerManager.ClearAllEssence();
    }

    public override void OnHitAdd(HitContext ctx)
    {
        float dmg = ctx._damageValue * _bonusPercentage;
        ctx._damageValue += (int)dmg;
    }
}
