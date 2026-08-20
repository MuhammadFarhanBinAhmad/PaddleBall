using UnityEngine;

public class ToxicSmokeAbility : ABSAbility
{
    protected ToxicEffectPool _toxicPool;

    private void Start()
    {
        _toxicPool = FindAnyObjectByType<ToxicEffectPool>();
    }

    public override void OnHitResolved(HitContext ctx)
    {
        if (_toxicPool == null) return;

        GameObject explosionGO = _toxicPool.GetObject();
        explosionGO.transform.position = ctx._brick.transform.position;
    }
}
