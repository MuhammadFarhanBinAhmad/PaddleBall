using UnityEngine;

public class NukemAbility : ABSAbility, IFireContextModifier
{
    HotZonePool _hotZonePool;

    public override void OnHitResolved(HitContext ctx)
    {
        //Instant kill brick
        ctx._brick.OnDamage(999);
    }

    public void ModifyFireContext(HitContext hitCtx, ref HotZoneArea hza)
    {
        float dmg = hitCtx._damageValue * _SOAbilityEffect._damagePerStackMultiplier;
        hza.SetStats((int)dmg, _SOAbilityEffect._stackLifeTime, _SOAbilityEffect._scaleSizeMultiplier);

    }
}
