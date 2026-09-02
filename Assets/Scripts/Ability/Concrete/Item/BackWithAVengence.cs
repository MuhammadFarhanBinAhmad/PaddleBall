using UnityEngine;

public class BackWithAVengence : ABSAbility, IExplosionContextModifier, IDischargeContextModifier, ICriticalContextModifier, IToxicContextModifier
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
            float dmg = ctx._damageValue * _SOAbilityEffect._baseDamageMultiplier;
            ctx._damageValue = (int)(dmg);
        }

    }

    public void ModifyExplosionContextAdd(HitContext hitCtx, ExplosionContext explosionCtx)
    {
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, ExplosionContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, ExplosionContext explosionCtx)
    {
        if (_vengenceInEffect)
            explosionCtx._Stats[STATID.BASE_DAMAGE] = (int)(explosionCtx._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, ExplosionContext explosionCtx)
    {
    }

    public void ModifyDischargeAdd(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeSubtract(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyDischargeMultiple(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        if (_vengenceInEffect)
            dischargeCtx._Stats[STATID.BASE_DAMAGE] = (int)(dischargeCtx._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);

    }

    public void ModifyDischargeDivide(HitContext hitCtx, AbilityContext dischargeCtx)
    {
    }

    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextMultiply(HitContext hitCtx, AbilityContext critContext)
    {
        if (_vengenceInEffect)
            critContext._Stats[STATID.BASE_DAMAGE] = (int)(critContext._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);

    }

    public void ModifyCriticalContextDivide(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextMultiple(AbilityContext toxicContext)
    {
        if (_vengenceInEffect)
            toxicContext._Stats[STATID.BASE_DAMAGE] = (int)(toxicContext._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier);

    }

    public void ModifyToxicContextDivide(AbilityContext toxicContext)
    {
    }
}
