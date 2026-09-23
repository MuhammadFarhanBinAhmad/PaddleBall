using UnityEngine;

public class OffensiveShield : ABSAbility, ICriticalContextModifier, IExplosionContextModifier, IToxicContextModifier, IDischargeContextModifier
{

    private void Start()
    {
        DeadZone _deadZone;
        _deadZone = FindAnyObjectByType<DeadZone>();
        _deadZone.MultipleMinusShieldValue(_SOAbilityEffect._shieldMultiplier);
        _deadZone.ResetShield();
        FindAnyObjectByType<ShieldUIManager>().UpdateShieldUI();
    }

    public void ModifyCriticalContextAdd(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextSubtract(HitContext hitCtx, AbilityContext critContext)
    {
        float dmg = hitCtx._damageValue * _SOAbilityEffect._baseDamageMultiplier;
        hitCtx._damageValue += (int)dmg;
    }

    public void ModifyCriticalContextMultiply(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyCriticalContextDivide(HitContext hitCtx, AbilityContext critContext)
    {
    }

    public void ModifyExplosionContextAdd(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextSubtract(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyExplosionContextMultiply(HitContext hitCtx, AbilityContext explosionCtx)
    {
        float dmg = explosionCtx._Stats[STATID.BASE_DAMAGE] * _SOAbilityEffect._baseDamageMultiplier;
        explosionCtx._Stats[STATID.BASE_DAMAGE] += dmg;
    }

    public void ModifyExplosionContextDivide(HitContext hitCtx, AbilityContext explosionCtx)
    {
    }

    public void ModifyToxicContextAdd(AbilityContext toxicContext)
    {
        float dmg = toxicContext._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._baseDamageMultiplier;
        toxicContext._Stats[STATID.DAMAGE_PER_STACK] += dmg;
    }

    public void ModifyToxicContextSubtract(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextMultiple(AbilityContext toxicContext)
    {
    }

    public void ModifyToxicContextDivide(AbilityContext toxicContext)
    {
    }

    public void ModifyDischargeAdd(HitContext hitCtx = null, AbilityContext dischargeCtx = null)
    {
    }

    public void ModifyDischargeSubtract(HitContext hitCtx = null, AbilityContext dischargeCtx = null)
    {
    }

    public void ModifyDischargeMultiple(HitContext hitCtx = null, AbilityContext dischargeCtx = null)
    {
        float dmg = dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._baseDamageMultiplier;
        dischargeCtx._Stats[STATID.DAMAGE_PER_STACK] += dmg;
    }

    public void ModifyDischargeDivide(HitContext hitCtx = null, AbilityContext dischargeCtx = null)
    {
    }
}
