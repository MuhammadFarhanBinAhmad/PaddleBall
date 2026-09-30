using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AbilityObject
{
    public ABILITY _ability;
    public GameObject _gameObject;
}
public class BrickAbilityManager : MonoBehaviour
{
    
    public List<AbilityObject> _abilityList = new List<AbilityObject>();

    SO_AggresiveBrickStats _AggresiveBrickStats;

    public void SetAggresiveBrickStats(SO_AggresiveBrickStats stats) => _AggresiveBrickStats = stats;

    public void ActivateAbilities(ABILITY _ABS)
    {
        foreach (AbilityObject ability in _abilityList)
        {
            bool isActive = (_ABS & ability._ability) != 0;

            ability._gameObject.SetActive(isActive);
            if ((ability._ability & ABILITY.SHOOT_PROJECTILE) != 0)
            {
                ability._gameObject.GetComponent<B_ShootProjectileAbility>().SetStats(_AggresiveBrickStats);
            }
        }


    }
}
