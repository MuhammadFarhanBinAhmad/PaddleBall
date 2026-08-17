using System;
using System.Collections.Generic;
using UnityEngine;

public class boss_TheApprenticeManager : BaseBossBrick
{
    public override void Start()
    {
        base.Start();
        _onTakingDamage += DamageFeedback;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        _onTakingDamage -= DamageFeedback;
    }

    internal override void HandleDamage(int damage)
    {
        _brickHealthComponent.ModifyHealth(-damage);
        _onTakingDamage?.Invoke();
        GlobalEffects.Instance.PlayLerpObject(_bossBody.gameObject, _damageAnim);

        if (_brickHealthComponent.GetHealth() <= 0)
        {
            print(_brickHealthComponent.GetHealth());
            BossDeathEvent();
        }
    }

    internal override void BossDeathEvent()
    {
        onEndBossFight?.Invoke();
        _bossManager.OnBossDeath(this);
    }
    internal override void GameOverBossEvent()
    {
        onGameOverBossFight?.Invoke();
        _bossManager.OnGameOver(this);
    }
}
