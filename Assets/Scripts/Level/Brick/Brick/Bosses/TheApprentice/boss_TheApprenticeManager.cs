using System;
using System.Collections.Generic;
using UnityEngine;

public class boss_TheApprenticeManager : BaseBossBrick
{
    public ParticleSystem _hitEffect;
    public Action _onTakingDamage;

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
        DamageFeedback();

        if (_brickHealthComponent.GetHealth() <= 0)
        {
            BossDeathEvent();
        }
    }
    internal override void DamageFeedback()
    {
        GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
        _hitEffect.Play();
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
