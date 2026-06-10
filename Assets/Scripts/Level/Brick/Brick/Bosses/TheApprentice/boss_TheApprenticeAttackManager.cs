using System.Collections;
using UnityEngine;

public class boss_TheApprenticeAttackManager : BaseBossAttackManager
{
    public Transform _spawnPos;
    public Transform _target;
    [Header("Attack One")]
    [SerializeField] GameObject _projectile;
    public float _attackOneAttackDuration;
    public float _attackOneShotInterval;
    [Header("Attack Two")]
    [SerializeField] GameObject _magicProjectile;
    public float _attackTwoAttackDuration;
    public float _attackTwoShotInterval;

    public override void AttackPatternOne()
    {
        Debug.Log("Attack One: continuous shooting");
        BeginTimedAttack(_attackOneAttackDuration); // attack lasts 5 seconds
        StartCoroutine(ShootNormalProjectile());
    }

    public override void AttackPatternTwo()
    {
        //Debug.Log("Attack One: projectile count based");
        //BeginHitCountAttack(3);
        //// Spawn your projectiles here
        //// Each projectile hit should call RegisterAttackHit()
        Debug.Log("Attack Two: continuous shooting");
        BeginTimedAttack(_attackTwoAttackDuration); // attack lasts 5 seconds
        StartCoroutine(ShootMagicProjectile());
    }

    public override void AttackPatternThree()
    {
        Debug.Log("Attack Three: manual");
        BeginManualAttack();
        // Call CompleteCurrentAttack() when your custom condition is met
        CompleteCurrentAttack();
    }

    public override void RestToNeutral()
    {
        Debug.Log("Boss is resting / resetting to neutral");
    }

    IEnumerator ShootNormalProjectile()
    {
        while (IsAttackActive)
        {
           
            GameObject proj = Instantiate(_projectile, _spawnPos.transform.position, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            ep.ShootProjectile(_target);
            yield return new WaitForSeconds(_attackOneShotInterval);
        }
    }
    IEnumerator ShootMagicProjectile()
    {
        while (IsAttackActive)
        {
            yield return new WaitForSeconds(_attackTwoShotInterval);
            GameObject proj = Instantiate(_magicProjectile, _spawnPos.transform.position, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            ep.ShootProjectile(_target);
        }
    }

}
