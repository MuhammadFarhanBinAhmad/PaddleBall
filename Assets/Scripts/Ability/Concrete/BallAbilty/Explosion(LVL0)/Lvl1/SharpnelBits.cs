using Unity.Mathematics.Geometry;
using UnityEngine;

public class SharpnelBits : MonoBehaviour
{
    BrickHealthComponent _ignoredBrick;

    [SerializeField]Ball _ball;
    [SerializeField]Rigidbody2D _rb;

    int _damage;
    [SerializeField] float _damageMultiplier;
    [SerializeField] float _lifetime;
    [SerializeField] float minImpulse;
    [SerializeField] float maxImpulse;

    bool _canDamage;
    [SerializeField] float _collisionDelay = 0.05f;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ball = FindAnyObjectByType<Ball>();
    }
    public void SetStats()
    {
        CancelInvoke();

        _canDamage = false;

        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        Vector2 dir = Random.insideUnitCircle.normalized;
        float mag = Random.Range(minImpulse, maxImpulse);

        _rb.AddForce(dir * mag, ForceMode2D.Impulse);

        _damage = Mathf.FloorToInt(_ball.GetBallBaseDamage() * _damageMultiplier);

        Invoke(nameof(EnableDamage), _collisionDelay);
        Invoke(nameof(KillObject), _lifetime);
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

        bb.OnDamage(_damage);

        CancelInvoke();

        KillObject();
    }
    void KillObject()
    {
        CancelInvoke();
        Debug.Log("Killed by Invoke at " + Time.time);

        gameObject.SetActive(false);
    }
}
