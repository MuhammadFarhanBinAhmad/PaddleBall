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
        StatusInstance si = ctx._health.GetStatusInstance(STATUSTYPE.TOXIC);
        if (si.stacks == si.maxStacks)
        {
            if (_explosionPool == null) return;


            si.stacks = 0;
            ctx._health.RemoveStatusVFX(STATUSTYPE.TOXIC);

            GameObject explosionGO = _explosionPool.GetObject();

            var ed = explosionGO.GetComponent<ToxicExplosionObject>();
            if (ed == null) return;

            ExplosionContext ectx = new ExplosionContext
            {
            };
            ectx._Stats[STATID.BASE_DAMAGE] = _SOAbilityEffect._abilityBaseDamageValue;
            ectx._Stats[STATID.SCALE_MULTIPLIER] = _SOAbilityEffect._scaleSizeMultiplier;

            // Let other abilities modify the explosion data
            _abilityManager.ApplyToxicModifiers(ectx);
            ed.Initialize(ctx, this);
        }


    }
}
