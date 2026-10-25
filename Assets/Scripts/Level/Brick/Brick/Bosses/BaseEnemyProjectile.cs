using System.Collections;
using UnityEngine;

public class BaseEnemyProjectile : MonoBehaviour
{
    [SerializeField] float _timeBeforeDeactive;
    internal float _shootSpeed;
    internal int _damage;

    public virtual void SetUpProjectile(float speed, int damage)
    {
        _shootSpeed = speed;
        _damage = damage;
    }
    public virtual void HandleProjectileDeath()
    {
        _damage = 0;
        _shootSpeed = 0;
        gameObject.SetActive(false);
    }
    public virtual IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(_timeBeforeDeactive);

        HandleProjectileDeath();
    }
    public int GetDamage() => _damage;
}
