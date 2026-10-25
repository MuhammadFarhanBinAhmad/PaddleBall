using UnityEngine;

public class Apprentice_FallingProjectile : BaseEnemyProjectile
{

    [SerializeField] int _meteorDamage;
    [SerializeField] float _fallSpeed;

    private void OnEnable()
    {
        StartCoroutine(DeactivateAfterTime());
        _damage = _meteorDamage;
        _shootSpeed = _fallSpeed;
    }
    private void Update()
    {
        transform.position += Vector3.down * _shootSpeed * Time.deltaTime;

    }
    public override void HandleProjectileDeath()
    {
        Destroy(gameObject);
    }

}
