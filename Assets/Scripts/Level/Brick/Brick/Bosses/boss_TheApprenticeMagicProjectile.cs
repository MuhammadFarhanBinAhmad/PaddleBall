using UnityEngine;

public class boss_TheApprenticeMagicProjectile : EnemyProjectile
{

    [SerializeField]Transform _target;

    public override void SetUpProjectile()
    {
        base.SetUpProjectile();
        _target = FindAnyObjectByType<boss_TheApprenticeShieldManager>().transform;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.GetComponent<boss_TheApprenticeShieldManager>() != null)
        {
            boss_TheApprenticeShieldManager _shield = other.gameObject.GetComponent<boss_TheApprenticeShieldManager>();
            _shield._onShieldDown?.Invoke();
            Destroy(gameObject);

        }
        if (other.gameObject.CompareTag("Ball"))
        {
            //Aim back to boss
            ShootProjectile(_target);
        }
    }

}
