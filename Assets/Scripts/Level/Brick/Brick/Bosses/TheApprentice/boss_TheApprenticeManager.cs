using System;
using UnityEngine;

public class boss_TheApprenticeManager : BaseBossBrick
{
    boss_TheApprenticeAttackManager _theApprenticeAttackManager;
    
    public ParticleSystem _hitEffect;

    public Action _onTakingDamage;
    public Action _onDeath;


    private void Start()
    {
        _onTakingDamage += DamageFeedback;
    }
    private void OnDestroy()
    {
        _onTakingDamage -= DamageFeedback;
    }

    internal override void HandleDamage(int damage)
    {
        _brickHealthComponent.ModifyHealth(-damage);
        _onTakingDamage?.Invoke();
        DamageFeedback();
    }
    internal override void DamageFeedback()
    {
        GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
        _hitEffect.Play();
    }

}
