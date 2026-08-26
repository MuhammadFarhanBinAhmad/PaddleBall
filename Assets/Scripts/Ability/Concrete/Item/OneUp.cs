using UnityEngine;

public class OneUp : ABSAbility
{
    public override void ModifyBaseValue(HitContext ctx)
    {
        ctx._damageValue++;
    }
}
