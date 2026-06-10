using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]SO_EnemyProjectile _soEnemyProjectile;

    Rigidbody2D _rigidbody2D;

    float _shootSpeed;
    float _damage;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    void SetUpProjectile()
    {
        _shootSpeed = _soEnemyProjectile._shootSpeed;
        _damage = _soEnemyProjectile._damage;
    }
    public void ShootProjectile(Transform target)
    {
        if (target == null) return;

        SetUpProjectile();

        Vector2 direction = (target.position - transform.position).normalized;
        ShootProjectile(direction);
    }

    public void ShootProjectile(Vector2 direction)
    {
        if (_rigidbody2D == null) return;

        direction = direction.normalized;

        _rigidbody2D.linearVelocity = direction * _shootSpeed;

        if (direction.sqrMagnitude > 0.0001f)
            transform.up = direction;
    }
    public void HandleProjectileDeath()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Vector2 avgNormal = Vector2.zero;
            int contacts = Mathf.Max(1, other.contactCount);
            for (int i = 0; i < other.contactCount; i++)
            {
                avgNormal += other.GetContact(i).normal;
            }
            avgNormal /= contacts;

            if (avgNormal.sqrMagnitude > 0.0001f)
                avgNormal.Normalize();
            else
                avgNormal = Vector2.up; // fallback

            Vector2 opposite = -avgNormal;
            transform.up = opposite;
        }
    }
}
