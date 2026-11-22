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

    SO_BrickAbilityStats _BrickAbilityStats;

    public void SetBrickAbilityStats(SO_BrickAbilityStats stats) => _BrickAbilityStats = stats;


    public void ActivateAbilities(ABILITY selectedAbilities)
    {
        if (_BrickAbilityStats == null)
        {
            Debug.LogError($"{name}: BrickAbilityStats is NULL!");
            return;
        }

        foreach (AbilityObject ability in _abilityList)
        {
            if (ability == null || ability._gameObject == null)
                continue;

            GameObject abilityObject = ability._gameObject;

            // Check whether THIS ability should be active.
            bool isActive =
                (selectedAbilities & ability._ability) != 0;

            // Enable or disable the GameObject.
            if (abilityObject.activeSelf != isActive)
                abilityObject.SetActive(isActive);

            // Skip abilities that were not selected.
            if (!isActive)
                continue;

            // Shooting ability.
            if ((ability._ability & ABILITY.SHOOT_PROJECTILE) != 0)
            {
                if (abilityObject.TryGetComponent(
                    out B_ShootProjectileAbility shooting))
                {
                    shooting.SetStats(_BrickAbilityStats);
                }
                else
                {
                    Debug.LogWarning(
                        $"{abilityObject.name}: Missing B_ShootProjectileAbility!"
                    );
                }
            }

            // Defensive ability.
            if ((ability._ability & ABILITY.DEFENSE) != 0)
            {
                if (abilityObject.TryGetComponent(
                    out B_ShieldAbility shield))
                {
                    shield.SetStats(_BrickAbilityStats);
                }
                else
                {
                    Debug.LogWarning(
                        $"{abilityObject.name}: Missing B_ShieldAbility!"
                    );
                }
            }
        }
    }


    public void ResetAllAbility()
    {
        foreach (AbilityObject ability in _abilityList)
        {
            ability._gameObject.SetActive(false);
        }
    }
}
