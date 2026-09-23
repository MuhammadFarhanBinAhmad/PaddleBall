using UnityEngine;

public class HotZoneAbility : ABSAbility
{
    protected HotZonePool _hotZonePool;

    private void Start()
    {
        _hotZonePool = FindAnyObjectByType<HotZonePool>();
    }

    public override void OnHitResolved(HitContext ctx)
    {
        if (_hotZonePool == null) return;

        GameObject fireGO = _hotZonePool.GetObject();
        fireGO.transform.position = ctx._brick.transform.position;

        float dmg = ctx._damageValue * _SOAbilityEffect._damagePerStackMultiplier;
        fireGO.GetComponent<HotZoneArea>().SetStats((int)dmg, _SOAbilityEffect._stackLifeTime);

        HotZoneArea area = fireGO.GetComponent<HotZoneArea>();

        _abilityManager.ApplyFireModifiers(ctx, ref area);
    }
}
