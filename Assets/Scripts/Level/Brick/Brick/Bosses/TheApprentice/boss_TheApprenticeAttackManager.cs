using System.Collections;
using UnityEngine;

enum APPRENTICECURRENTATTACK
{
    CONJURING,
    MOVE_AND_SHOOTING,
    SPAWNING_SHIELD,
    SPAWNING_COPIES
}


public class boss_TheApprenticeAttackManager : BaseBossAttackManager
{
    [Header("Apprentice")]
    [SerializeField] private Transform _bossTransform;
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private BrickAbilityManager _brickAbilityManager;

    [Header("Teleport")]
    [SerializeField] private SO_BrickSpecialEffect _lerpInEffect;
    [SerializeField] private SO_BrickSpecialEffect _lerpOutEffect;

    private void Start()
    {
        //TEST
        StartBossFight();
    }
    protected override IEnumerator MoveToRestPosition()
    {
        _brickAbilityManager.ResetAllAbility();

        // Disappear.
        GlobalEffects.Instance.PlayLerpObject(
            _bossTransform.gameObject,
            _lerpInEffect
        );

        yield return new WaitForSeconds(
            _lerpInEffect._lerpDuration + 0.25f
        );

        if (_stopAttacking || _isStunned)
            yield break;

        // Teleport home.
        _bossTransform.position = _spawnPos.position;

        // Reappear.
        GlobalEffects.Instance.PlayLerpObject(
            _bossTransform.gameObject,
            _lerpOutEffect
        );

        yield return new WaitForSeconds(
            _lerpOutEffect._lerpDuration + 0.25f
        );
    }

    protected override void OnBossStunned()
    {
        _brickAbilityManager.ResetAllAbility();

        Debug.Log("Apprentice stunned!");
    }

    public override void StopBossAttack()
    {
        base.StopBossAttack();

        _brickAbilityManager.ResetAllAbility();
    }
}
