using UnityEngine;

public class SharpnelAbility : ABSAbility
{
    SharpnelPool _sharpnelPool;

    Transform _transform;

    private void Start()
    {
        _sharpnelPool = FindAnyObjectByType<SharpnelPool>();

    }
    public override void OnHit(HitContext ctx)
    {
        _transform = ctx._health.transform;
    }
    public override void OnHitResolved(HitContext ctx)
    {
        for (int i=0; i < _SOAbilityEffect._amountToSpawn; i++)
        {
            GameObject explosion = _sharpnelPool.GetObject();
            explosion.transform.position = _transform.position;
            explosion.GetComponent<SharpnelBits>().SetStats(ctx._damageValue);
        }

    }
}
