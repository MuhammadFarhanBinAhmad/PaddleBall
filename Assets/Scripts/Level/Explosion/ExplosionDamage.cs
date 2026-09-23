using NUnit.Framework.Internal;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{

    AbilityContext _ctx;
    AbilityManager _manager;
    protected ExplosionPool _explosionPool;

    Vector3 _startScale = Vector3.one;
    SOStatusEffect _SOStatusEffect;

    [SerializeField]SO_FeedbackEffect _explosionEffect;

    int _damage;
    bool _hasExploded = false;

    [Header("Damage")]
    [SerializeField] private float _damageRadius;
    float _radiusModifierMultiplier = 1;
    [SerializeField] private LayerMask _brickLayer;
    // Cached collider buffer
    private readonly Collider2D[] _hits = new Collider2D[32];
    [SerializeField] ContactFilter2D _filter;
    STATUSTYPE _statusType;
    public GameObject [] effect ;

    private void Awake()
    {
        _manager = FindAnyObjectByType<AbilityManager>();
        _explosionPool = FindAnyObjectByType<ExplosionPool>();

        _filter.SetLayerMask(_brickLayer);
        _filter.useLayerMask = true;
        _filter.useTriggers = true;
    }

    public void Initialize(AbilityContext ctx,bool explodeNow)
    {
        _ctx = ctx;
        transform.position = _ctx._position;
        transform.localScale = _startScale  * _ctx._Stats[STATID.SCALE_MULTIPLIER];
        _radiusModifierMultiplier = _ctx._Stats[STATID.SCALE_MULTIPLIER];
        _damage = (int)ctx._Stats[STATID.BASE_DAMAGE];
        _damageRadius = ctx._Stats[STATID.EXPLOSION_RADIUS];
        _SOStatusEffect = ctx._statusEffect;
        _hasExploded = false;
        _statusType = ctx._statusType;
        foreach (var item in effect)
        {
            item.transform.localScale = transform.localScale;
        }
        if(explodeNow)
        ExplodeNow();
    }
    public void ExplodeNow()
    {
        if (_hasExploded) return;
        _hasExploded = true;
        Explosion();
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBombExplosion,transform.position);
        GlobalFeedbackManager.Instance.SetFeedbackValue(_explosionEffect);
        GlobalFeedbackManager.Instance.PlayGlobalFeedback();
        Invoke("DisableSelf", .5f);
        _radiusModifierMultiplier = 1;
    }
    void DisableSelf() => gameObject.SetActive(false);

    void Explosion()
    {

        int count = Physics2D.defaultPhysicsScene.OverlapCircle(
        transform.position,
        _damageRadius,
        _filter,
        _hits);

        Debug.Log($"Explosion detected {count} colliders.");

        for (int i = 0; i < count; i++)
        {
            Collider2D col = _hits[i];

            Debug.Log(
                $"Hit {i}: {col.name} | " +
                $"Layer: {LayerMask.LayerToName(col.gameObject.layer)}"
            );

            BrickBar brick = col.GetComponentInParent<BrickBar>();

            if (brick == null)
                continue;

            brick._brickHealthComponent.OnDamage(
                _damage,
                STATUSTYPE.EXPLOSION
            );
        }



        //for (int i = 0; i < count; i++)
        //{
        //    Collider2D col = _hits[i];
        //    print( col );
        //    BrickBar brick = col.GetComponentInParent<BrickBar>();

        //    if (brick == null)
        //        continue;

        //    brick._brickHealthComponent.OnDamage(
        //        _damage,
        //        STATUSTYPE.EXPLOSION
        //    );

        //    if (_ctx._abilityContext.Count == 0)
        //        continue;

        //    //foreach (var abctx in _ctx._abilityContext)
        //    //{
        //    //    switch (abctx.Key)
        //    //    {
        //    //        case STATUSTYPE.DISCHARGE:
        //    //            {
        //    //                _manager.ApplyDischargeModifiers(null, abctx.Value);
        //    //                brick._brickHealthComponent.ApplyStatus(abctx.Value, abctx.Key);
        //    //                continue;
        //    //            }
        //    //        case STATUSTYPE.EXPLOSION:
        //    //            {
        //    //                _manager.ApplyExplosionModifiers(null, abctx.Value);
        //    //                brick._brickHealthComponent.ApplyStatus(abctx.Value, abctx.Key);
        //    //                continue;
        //    //            }
        //    //    }
        //    //}

        //}


        //if (_statusType == STATUSTYPE.REMNANT)
        //{
        //    for (int i = 0; i < count; i++)
        //    {
        //        Collider2D col = _hits[i];
        //        if (_explosionPool == null) return;

        //        GameObject explosionGO = _explosionPool.GetExplosion();
        //        explosionGO.transform.position = col.gameObject.transform.position;

        //        var ed = explosionGO.GetComponent<ExplosionDamage>();
        //        if (ed == null) return;

        //        AbilityContext ectx = new AbilityContext
        //        {
        //            _source = gameObject,
        //            _position = col.gameObject.transform.position,
        //            _statusType = STATUSTYPE.EXPLOSION
        //        };
        //        ectx._Stats[STATID.BASE_DAMAGE] = _damage;
        //        ectx._Stats[STATID.SCALE_MULTIPLIER] = _radiusModifierMultiplier;

        //        // Let other abilities modify the explosion data
        //        //_abilityManager.ApplyExplosionModifiers(_hitContext, ectx);
        //        ed.Initialize(ectx, true);
        //    }
        //}
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position,
            _damageRadius
        );
    }
#endif
    
}
