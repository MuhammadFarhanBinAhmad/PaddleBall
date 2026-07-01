using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseBossBrick : MonoBehaviour
{
    internal BossManager _bossManager;
    TimeManager _timeManager;

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

    public virtual void Awake()
    {
        _episodeTitleCardUI = FindAnyObjectByType<EpisodeTitleCardUI>();
        _bossManager = FindAnyObjectByType<BossManager>();
        _timeManager = FindAnyObjectByType<TimeManager>();

    }
    public virtual void Start()
    {
        onStartBossFight += _episodeTitleCardUI.PlayTitleCardAnim;
        onStartBossFight += _baseBossAttackManager.StartBossFight;
        onStartBossFight += _timeManager.StartDayTimer;

        onEndBossFight += _baseBossAttackManager.StopBossAttack;
        onEndBossFight += _timeManager.StopDayTimer;

        onGameOverBossFight += _baseBossAttackManager.StopBossAttack;
        onGameOverBossFight += _timeManager.StopDayTimer;
    }
    public virtual void OnDestroy()
    {
        onStartBossFight -= _episodeTitleCardUI.PlayTitleCardAnim;
        onStartBossFight -= _baseBossAttackManager.StartBossFight;
        onStartBossFight -= _timeManager.StartDayTimer;

        onEndBossFight -= _baseBossAttackManager.StopBossAttack;
        onEndBossFight -= _timeManager.StopDayTimer;

        onGameOverBossFight -= _baseBossAttackManager.StopBossAttack;
        onGameOverBossFight -= _timeManager.StopDayTimer;

    }
    public List<SO_CutSceneEventContent> GetStartCutSceneEvents() => new List<SO_CutSceneEventContent>(_startCutsceneEvents);
    public List<SO_CutSceneEventContent> GetDefeatCutSceneEvents() => new List<SO_CutSceneEventContent>(_defeatBossCutsceneEvents);
    public List<SO_CutSceneEventContent> GetGameOverCutSceneEvents() => new List<SO_CutSceneEventContent>(_gameoverCutsceneEvents);

    internal abstract void HandleDamage(int damage);
    internal abstract void DamageFeedback();
    internal abstract void BossDeathEvent();
    internal abstract void GameOverBossEvent();
}
