using UnityEngine;

public class PiercerAbility : ABSAbility
{
    public override void OnHit(HitContext ctx)
    {
        //Increase base damage
        var critChance = _SOAbilityEffect._baseCritChance;
        var layerToDestroy  = _SOAbilityEffect._layerToDestroy;
        bool isCrit = RNGService.RollCrit(critChance, _SOAbilityEffect._bonusPerFail);

        if (isCrit)
        {
            ctx._status = STATUSTYPE.CRIT;
            ctx._brick.OnDamageLayer(layerToDestroy);
        }

        return; // only one crit owner

    }
}
