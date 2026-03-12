using System;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public event Action<ExplosionContext> OnExploded;
    ExplosionContext _ctx;

    int _damage;
    bool _hasExploded = false;
    public void Initialize(ExplosionContext ctx)
    {
        _ctx = ctx;
        transform.position = _ctx._position;

        _hasExploded = false;

        ExplodeNow();
    }
    public void ExplodeNow()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        OnExploded?.Invoke(_ctx);

     
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_ctx == null) return;

        var brick = other.GetComponent<BrickBar>();
        if (brick != null)
        {
            brick.OnDamage(_ctx._damage);
        }
    }
}
