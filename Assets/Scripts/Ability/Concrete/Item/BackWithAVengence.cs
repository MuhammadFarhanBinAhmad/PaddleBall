using UnityEngine;

public class BackWithAVengence : ABSAbility
{

    PaddleHealth _paddleHealth;
    
    float _currentTime;
    bool _vengenceInEffect;

    private void Awake()
    {
        _paddleHealth = FindAnyObjectByType<PaddleHealth>();
        _paddleHealth.OnPaddleEnable += ResetTime;
    }
    private void OnDestroy()
    {
        _paddleHealth.OnPaddleEnable -= ResetTime;
    }

    void ResetTime()
    {
        _currentTime = _SOAbilityEffect._timer;
        _vengenceInEffect = true;
    }

    private void Update()
    {
        if (!_vengenceInEffect)
            return;

        if(_currentTime > 0)
            _currentTime -= Time.deltaTime;
        else
            _vengenceInEffect = false;
    }

    public override void OnHitAdd(HitContext ctx)
    {
        if (_vengenceInEffect)
        {
            float dmg = ctx._damageValue;
            ctx._damageValue = (int)(_SOAbilityEffect._baseDamageMultiplier * dmg);
        }

    }
}
