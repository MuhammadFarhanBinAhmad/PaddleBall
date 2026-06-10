
public class RegrowthModifier : BrickModifierBase
{

    float _timer;
    float _life;

    public override void Initialize(BrickBar brick)
    {
        base.Initialize(brick);
        _timer = 0;
        
    }

    public override void Tick(float dt)
    {
        _timer += dt;

        if (_timer > _modifier._regenRate)
        {
            if (_brick._brickHealthComponent.GetHealth() < _brick._brickHealthComponent.GetStartingHealth())
            {
                _brick._brickHealthComponent.ModifyHealth(_modifier._regenValue);
            }

            if (_otherBricksModifiersList.Count > 0)
            {
                foreach (var bm in _otherBricksModifiersList)
                {
                    if(bm._brick._brickHealthComponent.GetHealth() < bm._brick._brickHealthComponent.GetStartingHealth())
                    {
                        bm._brick._brickHealthComponent.ModifyHealth(_modifier._regenValue);
                    }
                }
            }
        }
    }

}
