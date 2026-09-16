using Unity.Mathematics.Geometry;
using UnityEngine;

public class SharpnelBits : MonoBehaviour
{
    BrickHealthComponent _ignoredBrick;

    [SerializeField]Ball _ball;

    int _damage;
    [SerializeField] float _damageMultiplier;
    [SerializeField] float _lifetime;
    [SerializeField] float minImpulse;
    [SerializeField] float maxImpulse;

    bool _canDamage;
    [SerializeField] float _collisionDelay = 0.05f;

    Vector2 _velocity;

    private void Awake()
    {
        _ball = FindAnyObjectByType<Ball>();
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
    public void SetStats(int dmg)
    {
        CancelInvoke();

        _canDamage = false;

        _damage = Mathf.FloorToInt(dmg * _damageMultiplier);

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

        BrickHealthComponent bb = other.GetComponent<BrickHealthComponent>();

        if (bb == null)
            return;

        bb.OnDamage(_damage,STATUSTYPE.EXPLOSION);

        CancelInvoke();

        KillObject();
    }
    void KillObject()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
