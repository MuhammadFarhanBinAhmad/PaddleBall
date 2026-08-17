using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class BaseBossBrick : MonoBehaviour
{
    internal BossUIManager _bossUIManager;
    internal BossManager _bossManager;
    internal BaseBossFeedbackManager _bossFeedbackManager;
    TimeManager _timeManager;
    [SerializeField]SO_BossBaseStats _stats;

    public SO_BossIntroText _bossIntroText;
    public List<SO_CutSceneEventContent> _startCutsceneEvents;
    public List<SO_CutSceneEventContent> _defeatBossCutsceneEvents;
    public List<SO_CutSceneEventContent> _gameoverCutsceneEvents;

    [SerializeField] internal BrickHealthComponent _brickHealthComponent;
    [SerializeField] internal BaseBossAttackManager _baseBossAttackManager;
    EpisodeTitleCardUI _episodeTitleCardUI;

    public Action onStartBossFight;
    public Action onEndBossFight;
    public Action onGameOverBossFight;

    public ParticleSystem _hitEffect;
    [SerializeField] internal SO_BrickSpecialEffect _damageAnim;
    [SerializeField] internal Transform _bossBody;
    public Action _onTakingDamage;

    public virtual void Awake()
    {
        _episodeTitleCardUI = FindAnyObjectByType<EpisodeTitleCardUI>();
        _bossManager = FindAnyObjectByType<BossManager>();
        _bossUIManager = FindAnyObjectByType<BossUIManager>();
        _timeManager = FindAnyObjectByType<TimeManager>();
        _bossFeedbackManager = GetComponentInChildren<BaseBossFeedbackManager>();
    }
    public virtual void Start()
    {
        onStartBossFight += _episodeTitleCardUI.PlayTitleCardAnim;
        onStartBossFight += _baseBossAttackManager.StartBossFight;
        onStartBossFight += _timeManager.StartDayTimer;
        onStartBossFight += SetBossAttackStats;
        onStartBossFight += SetBossHealthUI;

        onEndBossFight += _baseBossAttackManager.StopBossAttack;
        onEndBossFight += _timeManager.StopDayTimer;
        onEndBossFight += _bossUIManager.CloseHealthUI;

        onGameOverBossFight += _baseBossAttackManager.StopBossAttack;
        onGameOverBossFight += _timeManager.StopDayTimer;

    }
    public virtual void OnDestroy()
    {
        onStartBossFight -= _episodeTitleCardUI.PlayTitleCardAnim;
        onStartBossFight -= _baseBossAttackManager.StartBossFight;
        onStartBossFight -= _timeManager.StartDayTimer;
        onStartBossFight -= SetBossAttackStats;
        onStartBossFight -= SetBossHealthUI;

        onEndBossFight -= _baseBossAttackManager.StopBossAttack;
        onEndBossFight -= _timeManager.StopDayTimer;
        onEndBossFight -= _bossUIManager.CloseHealthUI;

        onGameOverBossFight -= _baseBossAttackManager.StopBossAttack;
        onGameOverBossFight -= _timeManager.StopDayTimer;
    }
    void SetBossHealthUI()
    {
        _bossUIManager.OpenHealthUI();
        _brickHealthComponent.SetHealth(_stats._bossHealth);
        _bossUIManager.SetUpBossUI(_stats.name, _stats._bossHealth);
    }
    void SetBossAttackStats()
    {
        _baseBossAttackManager.SetSpeed(_stats._bossSpeed);
    }

    public List<SO_CutSceneEventContent> GetStartCutSceneEvents() => new List<SO_CutSceneEventContent>(_startCutsceneEvents);
    public List<SO_CutSceneEventContent> GetDefeatCutSceneEvents() => new List<SO_CutSceneEventContent>(_defeatBossCutsceneEvents);
    public List<SO_CutSceneEventContent> GetGameOverCutSceneEvents() => new List<SO_CutSceneEventContent>(_gameoverCutsceneEvents);

    internal abstract void HandleDamage(int damage);
    internal virtual void DamageFeedback()
    {
        GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
        _hitEffect.Play();
        _bossUIManager.UpdateBossHealthUI(_brickHealthComponent.GetHealth());
    }
    internal abstract void BossDeathEvent();
    internal abstract void GameOverBossEvent();
}
