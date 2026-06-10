using UnityEngine;

public class SpeedModifier : BrickModifierBase
{
    public override void Initialize(BrickBar brick)
    {
        base.Initialize(brick);
        brick._baseFallSpeed += _modifier._speedAdd;
        brick.RecalculateSpeed();
    }
}
