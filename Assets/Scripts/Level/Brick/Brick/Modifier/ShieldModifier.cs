using UnityEngine;

public class ShieldModifier : BrickModifierBase
{

    int _shieldHit;

    private void Start()
    {
        _shieldHit = _modifier._shieldValue;
        _circleCollider.isTrigger = false;
    }

    public override int ModifyIncomingDamage(int incomingDamage)
    {
        print("hit");
        _shieldHit--;
        if (_shieldHit < 0)
        {
            _brick.RemoveModifier(this);
            return 0;
        }
        return 0;
    }
    //I hate this. This break the rule
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            print("hit");
            _shieldHit--;
            if (_shieldHit < 0)
            {
                _brick.RemoveModifier(this);
            }
        }
    }
}
