using UnityEngine;

public class OneUp : ABSAbility
{
    public override void BeforeHitAdd(HitContext ctx)
    {
        ctx._damageValue++;
    }
}
