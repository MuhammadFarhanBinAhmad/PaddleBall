using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveAbility : ABSAbility
{

    ExplosionPool _explosionPool;

    private void Start()
    {
        _explosionPool = FindAnyObjectByType<ExplosionPool>();
    }

    public override void OnHit(HitContext ctx)
    {
        GameObject explosionGO = _explosionPool.GetExplosion();
        explosionGO.transform.position = transform.position;

        var ed = explosionGO.GetComponent<ExplosionDamage>();
        if (ed != null)
        {
            print("create new explosion");
            var ectx = new ExplosionContext
            {
                _damage = ctx._baseDamage,
                _source = gameObject,
                _position = ctx._brick.gameObject.transform.position,
            };
            ed.Initialize(ectx);
        }

        gameObject.SetActive(false);
    }

}
