using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class SharpnelBits : MonoBehaviour
{
    BrickHealthComponent _ignoredBrick;
    protected ExplosionPool _explosionPool;

    [SerializeField]Ball _ball;

    int _damage;
    [SerializeField] float _damageMultiplier;
    [SerializeField] float _lifetime;
    [SerializeField] float minImpulse;
    [SerializeField] float maxImpulse;

    bool _canDamage;
    [SerializeField] float _collisionDelay = 0.05f;

    Vector2 _velocity;
    STATUSTYPE _type;
    


    private void Awake()
    {
        _ball = FindAnyObjectByType<Ball>();
        _explosionPool = FindAnyObjectByType<ExplosionPool>();

    }
    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (_velocity.sqrMagnitude <= 0.000001f)
            return;

        transform.position +=
            (Vector3)(_velocity * Time.deltaTime);
    }
    public void SetStats(int dmg, STATUSTYPE type)
    {
        CancelInvoke();
        _canDamage = false;
        _damage = Mathf.FloorToInt(dmg * _damageMultiplier);
        _type = type;
        Impluse();
        Invoke(nameof(EnableDamage), _collisionDelay);
        Invoke(nameof(KillObject), _lifetime);
    }
    void Impluse()
    {
        // Reset movement state.
        _velocity = Vector2.zero;

        // Reproduce the old initial Rigidbody impulse
        // using our own velocity instead.
        Vector2 direction = Random.insideUnitCircle;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.up;

        direction.Normalize();

        float magnitude =
            Random.Range(minImpulse, maxImpulse);

        _velocity = direction * magnitude;
    }
    void EnableDamage()
    {
        _canDamage = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canDamage)
            return;
        if (other.GetComponent<BrickHealthComponent>() != null)
        {
            BrickHealthComponent bb = other.GetComponent<BrickHealthComponent>();

            if((_type & STATUSTYPE.CLUSTERBOMB) != 0)
            {
                GameObject explosionGO = _explosionPool.GetExplosion();
                explosionGO.transform.position = bb.transform.position;

                var ed = explosionGO.GetComponent<ExplosionDamage>();
                if (ed == null) return;

                ExplosionContext ectx = new ExplosionContext
                {
                    _source = gameObject,
                    _position = bb.transform.position,
                    _statusEffect = null
                };
                ectx._Stats[STATID.BASE_DAMAGE] = _damage;
                ectx._Stats[STATID.SCALE_MULTIPLIER] = .5f;

                // Let other abilities modify the explosion data
                ed.Initialize(ectx, true);
            }
            else
            {
                bb.OnDamage(_damage, STATUSTYPE.NONE);
            }

            CancelInvoke();
            KillObject();
        }
        
    }
    void KillObject()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
