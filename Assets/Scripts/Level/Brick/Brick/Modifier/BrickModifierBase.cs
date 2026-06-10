using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class BrickModifierBase : MonoBehaviour, IBrickModifier
{
    internal BrickBar _brick;
    [SerializeField] internal SOBrickModifier _modifier;
    internal List<BrickModifierBase> _otherBricksModifiersList =
        new List<BrickModifierBase>();
    float _cleanupTimer; 
    internal CircleCollider2D _circleCollider;
    Rigidbody2D _rigidbody2D;
    public virtual void Initialize(BrickBar brick)
    {
        _brick = brick;

        transform.SetParent(_brick.transform, false);
        transform.localPosition = Vector3.zero;

        _circleCollider = GetComponent<CircleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _circleCollider.radius = _modifier._aoeRadius;
        _circleCollider.isTrigger = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }

    public virtual int ModifyIncomingDamage(int incomingDamage) => incomingDamage;

    public virtual void OnDamageApplied(int appliedDamage)
    {
    }

    public virtual void OnRemove()
    {
        Destroy(this.gameObject);
    }

    public virtual void Tick(float dt)
    {
        _cleanupTimer += dt;

        if (_cleanupTimer >= 1f)
        {
            _cleanupTimer = 0f;
            _otherBricksModifiersList.RemoveAll(x => x == null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BrickModifierBase modifier))
        {
            if (!_otherBricksModifiersList.Contains(modifier))
                _otherBricksModifiersList.Add(modifier);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out BrickModifierBase modifier))
        {
            _otherBricksModifiersList.Remove(modifier);
        }
    }
    public int GetDayToUnlock() => _modifier._dayToUnlock;
}
