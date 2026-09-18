using UnityEngine;

public class SharpnelAbility : ABSAbility
{
    SharpnelPool _sharpnelPool;
    Transform _transform;

    bool _convertToBomb;

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
        if (_convertToBomb)
            ctx._status |= STATUSTYPE.CLUSTERBOMB;

        for (int i=0; i < _SOAbilityEffect._amountToSpawn; i++)
        {
            GameObject explosion = _sharpnelPool.GetObject();
            explosion.transform.position = _transform.position;
            explosion.GetComponent<SharpnelBits>().SetStats(ctx._damageValue, ctx._status);

        }
    }
    public void ConvertSharpnel() => _convertToBomb = true;
}
