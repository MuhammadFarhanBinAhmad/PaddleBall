
public class CrimsonMask : ABSAbility
{
    public override void OnHitResolved(HitContext ctx)
    {
        if(ctx._health.HasStatus(STATUSTYPE.BLEEDING))
        {
            if (ctx._health.GetStatusStack(STATUSTYPE.BLEEDING) >= ctx._health.GetMaxStack(STATUSTYPE.BLEEDING))
            {
                StatusInstance si = ctx._health.GetStatusInstance(STATUSTYPE.BLEEDING);
                si.stacks = 0;
                float damage = ctx._damageValue * _SOAbilityEffect._critMultiplier;
                ctx._health.OnDamage((int) damage);
            }
        }
    }
}
