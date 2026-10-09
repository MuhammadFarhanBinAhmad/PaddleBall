using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    AbilityContext _ctx;

    Vector3 _startScale = Vector3.one;

    [SerializeField] SO_FeedbackEffect _explosionEffect;

    int _damage;
    bool _hasExploded;

    [Header("Damage")]
    [SerializeField] float _damageRadius;
    float _radiusModifierMultiplier = 1f;

    [SerializeField] LayerMask _brickLayer;

    // Reusable collider buffer
    readonly Collider2D[] _hits = new Collider2D[64];

    // Reused between explosions
    readonly HashSet<BrickBar> _affectedBricks =
        new HashSet<BrickBar>(16);

    [SerializeField] ContactFilter2D _filter;

    STATUSTYPE _statusType;

    public GameObject[] effect;

    private void Awake()
    {
        _filter.SetLayerMask(_brickLayer);
        _filter.useLayerMask = true;
        _filter.useTriggers = true;
    }

    public void Initialize(AbilityContext ctx, bool explodeNow)
    {
        _ctx = ctx;

        transform.position = ctx._position;

        transform.localScale =
            _startScale *
            ctx._Stats[STATID.SCALE_MULTIPLIER];

        _radiusModifierMultiplier =
            ctx._Stats[STATID.SCALE_MULTIPLIER];

        _damage =
            (int)ctx._Stats[STATID.BASE_DAMAGE];

        print(_damage);

        _damageRadius =
            ctx._Stats[STATID.EXPLOSION_RADIUS];

        _hasExploded = false;
        _statusType = ctx._statusType;

        foreach (GameObject item in effect)
        {
            if (item != null)
                item.transform.localScale = transform.localScale;
        }

        if (explodeNow)
            ExplodeNow();
    }

    public void ExplodeNow()
    {
        if (_hasExploded)
            return;

        _hasExploded = true;

        Explosion();

        AudioManager.Instance.PlayOneShot(
            FmodEvent.Instance.sfx_onBombExplosion,
            transform.position
        );

        GlobalFeedbackManager.Instance.SetFeedbackValue(
            _explosionEffect
        );

        GlobalFeedbackManager.Instance.PlayGlobalFeedback();

        Invoke(nameof(DisableSelf), 0.5f);

        _radiusModifierMultiplier = 1f;
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    void Explosion()
    {
        int count =
            Physics2D.defaultPhysicsScene.OverlapCircle(
                transform.position,
                _damageRadius,
                _filter,
                _hits
            );

        _affectedBricks.Clear();

        for (int i = 0; i < count; i++)
        {
            Collider2D col = _hits[i];

            if (col == null)
                continue;

            BrickBar brick =
                col.GetComponentInParent<BrickBar>();

            if (brick == null)
                continue;

            if (!_affectedBricks.Add(brick))
                continue;

            brick._brickHealthComponent.OnDamage(
                _damage,
                STATUSTYPE.EXPLOSION
            );
        }
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