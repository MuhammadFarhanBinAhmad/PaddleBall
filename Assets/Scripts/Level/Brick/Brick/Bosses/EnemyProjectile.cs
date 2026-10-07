using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float _timeBeforeDeactive;
    [SerializeField] TrailRenderer _trail;
    Rigidbody2D _rigidbody2D;

    float _shootSpeed;
    int _damage;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        StartCoroutine(DeactivateAfterTime());
    }
    public virtual void SetUpProjectile(float speed, int damage)
    {
        _trail.Clear();
        _trail.enabled = true;
        _trail.Clear();
        _shootSpeed = speed;
        _damage = damage;
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
        _trail.enabled = false;
        _damage = 0;
        _shootSpeed = 0;
        gameObject.SetActive(false);
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

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(_timeBeforeDeactive);

        HandleProjectileDeath();
    }
    public int GetDamage() => _damage;
}
