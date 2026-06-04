
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
            if (_brick._health < _brick._startingHealth)
            {
                _brick._health += _modifier._regenValue;
            }

            if (_otherBricksModifiersList.Count > 0)
            {
                foreach (var bm in _otherBricksModifiersList)
                {
                    if(bm._brick._health < bm._brick._startingHealth)
                    {
                        bm._brick._health += _modifier._regenValue;
                    }
                }
            }
        }
    }

}
