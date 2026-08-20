using UnityEngine;

public class OneUp : ABSAbility
{
    public override void OnHitAdd(HitContext ctx)
    {
        ctx._damageValue++;
    }

}
