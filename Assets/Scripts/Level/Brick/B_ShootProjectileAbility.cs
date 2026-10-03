using System.Collections;
using UnityEngine;

public class B_ShootProjectileAbility : MonoBehaviour
{
    SO_BrickAbilityStats _stats;
    [Header("Projectile Basic Stats")]
    ProjectilePool _projectilePool;
    [SerializeField] Transform _projSpawnPoint;
    [SerializeField] float _shootInterval;
    [SerializeField] float _projectileSpeed;
    [SerializeField] int _damage;
    [Header("Projectile Special Stats")]
    [SerializeField] bool _burstFire;
    [SerializeField] bool _shotgunFire;
    [SerializeField] bool _homingShot;
    [SerializeField] bool _sniperlaserShot;

    [Header("Burst Fire Stats")]
    [SerializeField] float _burstInterval;
    [SerializeField] int _numberOfSuccession;
    [Header("Shotgun Fire Stats")]
    [SerializeField] int _numberOfProjectiles;
    [SerializeField] float _shotAngle;
    [Header("Sniper/Laser Stats")]
    [SerializeField] float _buildUpTime;
    [SerializeField] GameObject _buildUpVFX;
    [Header("ProjectileSpecialEffect")]

    [Header("Homing (subtle)")]
    [Range(0f, 1f)]
    [SerializeField] float _homingStrength;
    [SerializeField] float _minVerticalForHoming;
    [SerializeField] float _homingMaxDistance;


    Transform _target;

    private void Awake()
    {
        _projectilePool = FindAnyObjectByType<ProjectilePool>();
        _target = FindAnyObjectByType<PaddleHealth>().transform;
    }
    public void SetStats(SO_BrickAbilityStats stats)
    {
        _stats = stats;

        _shootInterval = _stats._shootInterval;
        _damage = _stats._damage;
        _burstFire = _stats._burstFire;
        _shotgunFire = _stats._shotgunFire;
        _homingShot = _stats._homingShot;
        _sniperlaserShot = _stats._sniperlaserShot;

        _burstInterval = _stats._burstInterval;
        _numberOfSuccession = _stats._numberOfSuccession;

        _numberOfProjectiles = _stats._numberOfProjectiles;
        _shotAngle = _stats._shotAngle;

        _buildUpTime = _stats._buildUpTime;
        _buildUpVFX = _stats._buildUpVFX;

    }

    private void OnEnable()
    {
        Invoke("StartShooting", 3);
    }

    void StartShooting()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            // =========================
            // BURST FIRE
            // =========================
            if (_burstFire)
            {
                for (int i = 0; i < _numberOfSuccession; i++)
                {
                    Shoot();

                    // Wait between each projectile in the burst
                    if (i < _numberOfSuccession - 1)
                        yield return new WaitForSeconds(_burstInterval);
                }

                // Wait before starting the next burst
                yield return new WaitForSeconds(_shootInterval);
            }
            else
            {
                // =========================
                // NORMAL / SHOTGUN
                // =========================


                if(_sniperlaserShot)
                {
                    GameObject vfx = Instantiate(_buildUpVFX, _projSpawnPoint.transform.position, Quaternion.identity);
                    vfx.transform.parent = _projSpawnPoint;
                    yield return new WaitForSeconds(_buildUpTime);
                    Shoot();
                    yield return new WaitForSeconds(_shootInterval);
                }
                else
                {
                    Shoot();
                    yield return new WaitForSeconds(_shootInterval);
                }

            }
        }
    }

    private void Shoot()
    {
        if (_projectilePool == null)
        {
            print("ProjectilePool is NULL!");
            return;
        }

        if (_target == null)
        {
            print("Target is NULL!");
            return;
        }

        // =========================
        // SHOTGUN
        // =========================
        if (_shotgunFire)
        {
            ShootShotgun();
            return;
        }

        // =========================
        // NORMAL SINGLE SHOT
        // =========================
        ShootProjectile(GetDirectionToTarget());

    }
    private void ShootShotgun()
    {
        Vector2 baseDirection = GetDirectionToTarget();

        // Only one projectile
        if (_numberOfProjectiles <= 1)
        {
            ShootProjectile(baseDirection);
            return;
        }

        // Total spread is divided around the center direction
        float startAngle = -_shotAngle / 2f;
        float angleStep = _shotAngle / (_numberOfProjectiles - 1);

        for (int i = 0; i < _numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + angleStep * i;

            Vector2 shotDirection =
                Quaternion.Euler(0f, 0f, currentAngle) * baseDirection;

            ShootProjectile(shotDirection);
        }
    }

    private void ShootProjectile(Vector2 direction)
    {
        GameObject p = _projectilePool.GetObject();

        if (!p.TryGetComponent(out EnemyProjectile ep))
        {
            Debug.LogError("Projectile does not have EnemyProjectile component!");
            return;
        }

        p.transform.position = _projSpawnPoint.position;
        ep.SetUpProjectile(_stats._projectileSpeed, _stats._damage);
        ep.ShootProjectile(direction);
    }

    private Vector2 GetDirectionToTarget()
    {
        return (
            (Vector2)_target.position -
            (Vector2)_projSpawnPoint.position
        ).normalized;
    }


}
