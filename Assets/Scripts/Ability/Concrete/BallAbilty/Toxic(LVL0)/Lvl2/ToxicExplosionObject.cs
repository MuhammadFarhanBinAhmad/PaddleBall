using UnityEngine;

public class ToxicExplosionObject : MonoBehaviour
{
    private AbilityManager _abilityManager;

    [SerializeField] private SOAbilityEffect _SOAbilityEffect;
    [SerializeField] private SO_FeedbackEffect _explosionEffect;

    [Header("Damage")]
    [SerializeField] private float _damageRadius;
    [SerializeField] private LayerMask _brickLayer;

    // Cached collider buffer
    private readonly Collider2D[] _hits = new Collider2D[32];
    private ContactFilter2D _filter;

    private bool _hasExploded = false;

    private ToxicContext _context;

    private void Awake()
    {
        _abilityManager = FindAnyObjectByType<AbilityManager>();

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_brickLayer);
        _filter.useTriggers = true;
    }

    public void Initialize(HitContext ctx, ABSAbility sourceAbility)
    {
        _hasExploded = false;

        transform.position = ctx._health.transform.position;

        _context = new ToxicContext
        {
            _abililty = sourceAbility,
            _statusType = _SOAbilityEffect._statusType
        };

        _context._Stats[STATID.STACKS_TO_ADD] =
            _SOAbilityEffect._stacksToAdd;

        _context._Stats[STATID.MAX_STACKS] =
            _SOAbilityEffect._maxStacks;

        _context._Stats[STATID.DAMAGE_PER_STACK] =
            _SOAbilityEffect._damagePerStack;

        _context._Stats[STATID.STACK_LIFETIME] =
            _SOAbilityEffect._stackLifeTime;

        _context._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE] =
            _SOAbilityEffect._timeBeforeEffectActivate;

        _abilityManager.ApplyToxicModifiers(_context);

        ExplodeNow();
    }

    
    public void ExplodeNow()
    {
        if (_hasExploded)
            return;

        _hasExploded = true;

        AudioManager.Instance.PlayOneShot(
            FmodEvent.Instance.sfx_onBombExplosion,
            transform.position
        );

        GlobalFeedbackManager.Instance.SetFeedbackValue(_explosionEffect);
        GlobalFeedbackManager.Instance.PlayGlobalFeedback();

        ApplyToxicToExplosionTargets();

        Invoke(nameof(DisableSelf), 0.5f);
    }

    private void ApplyToxicToExplosionTargets()
    {
        int count = Physics2D.defaultPhysicsScene.OverlapCircle(
            transform.position,
            _damageRadius,
            _filter,
            _hits
        );

        for (int i = 0; i < count; i++)
        {
            Collider2D col = _hits[i];

            if (!col.TryGetComponent(out BrickBar brick))
                continue;

            brick._brickHealthComponent.ApplyStatus(_context,STATUSTYPE.TOXIC);
        }
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
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