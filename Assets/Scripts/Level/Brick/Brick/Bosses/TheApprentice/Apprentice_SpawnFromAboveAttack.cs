using System.Collections;
using UnityEngine;

public class Apprentice_SpawnFromAboveAttack : BaseBossAttack
{
    [Header("Ability")]
    [SerializeField] private BrickAbilityManager _abilityManager;
    [SerializeField] private ABILITY _ability;
    [SerializeField] private SO_BrickAbilityStats _abilityStats;
    public GameObject _metorObject;

    protected override IEnumerator ExecuteAttack()
    {
        throw new System.NotImplementedException();
    }
    protected override void OnAttackCompleted()
    {
        _abilityManager?.ResetAllAbility();
    }
}
