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
        bool _spawn = RNGService.RollCrit(_SOAbilityEffect._baseChance + ctx._modifyChance, _SOAbilityEffect._bonusPerFail);

        if(_spawn)
        {
            GameObject explosionGO = _toxicPool.GetObject();
            explosionGO.transform.position = ctx._health.transform.position;
            ToxicSmokeObject _tso = explosionGO.GetComponent<ToxicSmokeObject>();
            _tso.Initialize();
        }

    }
}
