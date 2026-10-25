using System.Collections;
using UnityEngine;

public class Apprentice_FallingProjectile : MonoBehaviour
{
    [SerializeField] float _timeBeforeDeactive;
    [SerializeField] TrailRenderer _trail;

    [SerializeField] float _fallSpeed;
    int _damage;


    public void OnLight()
    {

    }
    private void Awake()
    {
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
        _damage = damage;
    }
    public void HandleProjectileDeath()
    {
        _trail.enabled = false;
        _damage = 0;
        Destroy(gameObject);
    }


    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Wall"))
    //    {
    //        Vector2 avgNormal = Vector2.zero;
    //        int contacts = Mathf.Max(1, other.contactCount);
    //        for (int i = 0; i < other.contactCount; i++)
    //        {
    //            avgNormal += other.GetContact(i).normal;
    //        }
    //        avgNormal /= contacts;

    //        if (avgNormal.sqrMagnitude > 0.0001f)
    //            avgNormal.Normalize();
    //        else
    //            avgNormal = Vector2.up; // fallback

    //        Vector2 opposite = -avgNormal;
    //        transform.up = opposite;
    //    }
    //}

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(_timeBeforeDeactive);

        HandleProjectileDeath();
    }
    public int GetDamage() => _damage;
}
