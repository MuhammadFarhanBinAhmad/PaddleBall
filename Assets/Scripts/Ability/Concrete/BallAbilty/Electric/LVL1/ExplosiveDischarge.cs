using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ExplosiveDischarge : ABSAbility, IModifyStackableAbility
{

    public void ModifyStackableAbility(AbilityContext _abc)
    {
        ExplosionContext ectx = new ExplosionContext
        {
            _source = gameObject,
            _statusEffect = null
        };
        ectx._Stats[STATID.BASE_DAMAGE] = _abc._Stats[STATID.DAMAGE_PER_STACK] * _SOAbilityEffect._damagePerStackMultiplier;
        ectx._Stats[STATID.EXPLOSION_RADIUS] = _SOAbilityEffect._explosionRadius ;

        _abc._explosionContext = ectx;

    }
}
