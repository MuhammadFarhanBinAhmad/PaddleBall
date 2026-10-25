using System.Collections;
using UnityEngine;


public class boss_TheApprenticeAttackManager : BaseBossAttackManager
{
    [Header("Apprentice")]
    [SerializeField] private BrickAbilityManager _brickAbilityManager;

    //protected override IEnumerator MoveToRestPosition()
    //{
    //    //_brickAbilityManager.ResetAllAbility();

    //    //// Disappear.
    //    //GlobalEffects.Instance.PlayLerpObject(
    //    //    _bossTransform.gameObject,
    //    //    _lerpInEffect
    //    //);

    //    //yield return new WaitForSeconds(
    //    //    _lerpInEffect._lerpDuration + 0.25f
    //    //);

    //    //if (_stopAttacking || _isStunned)
    //    //    yield break;

    //    //// Teleport home.
    //    //_bossTransform.position = _spawnPos.position;

    //    //// Reappear.
    //    //GlobalEffects.Instance.PlayLerpObject(
    //    //    _bossTransform.gameObject,
    //    //    _lerpOutEffect
    //    //);

    //    //yield return new WaitForSeconds(
    //    //    _lerpOutEffect._lerpDuration + 0.25f
    //    //);
    //}
}
