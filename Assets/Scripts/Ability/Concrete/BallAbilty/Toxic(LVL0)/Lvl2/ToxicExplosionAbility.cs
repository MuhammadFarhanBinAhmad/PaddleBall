using UnityEngine;

public class ToxicExplosionAbility : ABSAbility
{

    protected ToxicExplosionPool _explosionPool;

    private void Start()
    {
        _explosionPool = FindAnyObjectByType<ToxicExplosionPool>();
    }

    public override void OnHitResolved(HitContext ctx)
    {
        if (_explosionPool == null) return;

        GameObject explosionGO = _explosionPool.GetObject();

        var ed = explosionGO.GetComponent<ToxicExplosionObject>();
        if (ed == null) return;

        ExplosionContext ectx = new ExplosionContext
        {
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _SOAbilityEffect._abilityBaseDamageValue;
        ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

        // Let other abilities modify the explosion data
        _abilityManager.ApplyDischargeModifiers(ctx, ectx);
        ed.Initialize(ctx,this);
    }
}
