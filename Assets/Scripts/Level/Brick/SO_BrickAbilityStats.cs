using UnityEngine;

[System.Flags]
public enum SHIELD_SIDE
{
    None = 0,
    FRONT = 1 << 0,  // 0001 = 1
    LEFT = 1 << 1,  // 0010 = 2
    RIGHT = 1 << 2,  // 0100 = 4
    BACK = 1 << 3   // 1000 = 8
}

[CreateAssetMenu(fileName = "SO_BrickAbilityStats", menuName = "Enemy And Bosses /SO_BrickAbilityStats")]
public class SO_BrickAbilityStats : ScriptableObject
{

    //AbilityType
    public bool _aggresiveBrick;
    public bool _defensiveBrick;
    public bool _supportBrick;

    //Projectile Basic Stats
    public float _shootInterval;
    public int _damage;
    public float _projectileSpeed;
    public float _buildUpTime;
    public GameObject _buildUpVFX;

    //Projectile Special Stats
    public bool _burstFire;
    public bool _shotgunFire;
    public bool _homingShot;
    //Burst Fire Stats
    public float _burstInterval;
    public int _numberOfSuccession;
    //Shotgun Fire Stats
    public int _numberOfProjectiles;
    public float _shotAngle;

    //Defensive Brick stats
    //StaticShield
    public bool _activateStaticShield;
    public SHIELD_SIDE _shieldSide;
    //TimeShield
    public float _upTime;
    public float _rechargeTime;
    //MovingShield
    public bool _activateMovingShield;
    public float _movementSpeed;

    //[Header("ProjectileSpecialEffect")]
}
