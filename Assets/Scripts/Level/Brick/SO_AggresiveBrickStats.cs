using UnityEngine;

[CreateAssetMenu(fileName = "SO_AggresiveBrickStats", menuName = "Enemy And Bosses /SO_AggresiveBrickStats")]
public class SO_AggresiveBrickStats : ScriptableObject
{
    [Header("Projectile Basic Stats")]
    public float _shootInterval;
    public int _damage;
    public float _projectileSpeed;
    [Header("Projectile Special Stats")]
    public bool _burstFire;
    public bool _shotgunFire;
    public bool _homingShot;
    public bool _sniperlaserShot;
    [Header("Burst Fire Stats")]
    public float _burstInterval;
    public int _numberOfSuccession;
    [Header("Shotgun Fire Stats")]
    public int _numberOfProjectiles;
    public float _shotAngle;
    [Header("Sniper/Laser Stats")]
    public float _buildUpTime;
    public GameObject _buildUpVFX;
    //[Header("ProjectileSpecialEffect")]
}
