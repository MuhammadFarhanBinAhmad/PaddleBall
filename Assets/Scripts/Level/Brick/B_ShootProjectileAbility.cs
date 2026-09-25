using System.Collections;
using UnityEngine;

public class B_ShootProjectileAbility : MonoBehaviour
{
    [Header("Projectile")]
    ProjectilePool _projectilePool;
    [SerializeField] Transform _projSpawnPoint;

    Transform _target;

    [Header("Shooting")]
    [SerializeField] private float _shootInterval = 1f;
    private void Awake()
    {
        _projectilePool = FindAnyObjectByType<ProjectilePool>();
        _target = FindAnyObjectByType<PaddleHealth>().transform;
    }

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(_shootInterval);
        }
    }

    private void Shoot()
    {
        if (_projectilePool == null)
        {
            Debug.LogError("ProjectilePool is NULL!");
            return;
        }

        GameObject p = _projectilePool.GetObject();
        if (p.TryGetComponent(out EnemyProjectile ep))
        {
            if (_target == null)
            {
                Debug.LogError("Target is NULL!");
                return;
            }
            p.transform.position = _projSpawnPoint.position;
            ep.ShootProjectile(_target);
        }

    }
}
