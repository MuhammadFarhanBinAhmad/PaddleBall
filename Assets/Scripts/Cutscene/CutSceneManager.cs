using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class CutSceneManager : MonoBehaviour
{
    EpisodeTitleCardUI _episodeTitleCardUI;
    PaddleBallShooter _paddleBallShooter;
    PaddleMovement _paddleMovement;
    BallDirectionArrow _ballDirectionArrow;
    PaddleVacoom _paddleVacoom;
    BrickGenerator _brickGenerator;
    TimeManager _timeManager;
    [SerializeField] Ball _ball;

    public List<SO_CutSceneEventContent> _cutSceneEvent = new List<SO_CutSceneEventContent>();
    [SerializeField]GameObject _currentBossObject;

    [Header("EventType")]
    [SerializeField] TalkingBubbleCutsceneEvent _dialougeEvent;
    [SerializeField] PopInCutSceneEvent _popInCutSceneEvent;
    [SerializeField] ParticleEffectCutsceneEvent _particleEffectCutsceneEvent;
    [SerializeField] CamShakeCutSceneEvent _camshakeCutSceneEvent;
    [SerializeField] DestroyBossCutsceneEvent _destryoBossCutSceneEvent;

    BaseCutsceneEvent _currentEvent;
    [SerializeField] int _cutSceneIndex;

    public bool _isStartOfBossFight;

    public Action<bool> _setBoolOnStartCutScene;
    public Action<bool> _setBoolOnEndCutScene;
    private bool _cutSceneBatchResolved;
    private bool _advancingCutscene;

    public Action _onStartCutScene;
    public Action _onEndBossCutScene;

    int _activeCutsceneEventCount;

    private void Awake()
    {
        _episodeTitleCardUI = FindAnyObjectByType<EpisodeTitleCardUI>();
        _paddleBallShooter = FindAnyObjectByType<PaddleBallShooter>();
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _ballDirectionArrow = FindAnyObjectByType<BallDirectionArrow>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();
        _timeManager = FindAnyObjectByType<TimeManager>();
    }
    private void Start()
    {
        _onStartCutScene += _ball.ResettingBall;

        _setBoolOnStartCutScene += _paddleBallShooter.DisableShoot;
        _setBoolOnStartCutScene += _paddleMovement.DisblePaddleMovement;
        _setBoolOnStartCutScene += _ballDirectionArrow.DisableArrow;
        _setBoolOnStartCutScene += _paddleVacoom.DisableVacoom;

        _setBoolOnEndCutScene += _paddleBallShooter.DisableShoot;
        _setBoolOnEndCutScene += _paddleMovement.DisblePaddleMovement;
        _setBoolOnEndCutScene += _ballDirectionArrow.DisableArrow;
        _setBoolOnEndCutScene += _paddleVacoom.DisableVacoom;

        _onEndBossCutScene += _brickGenerator.StartFirstWaveOfEpisode;

    }
    private void OnDestroy()
    {
        _onStartCutScene -= _ball.ResettingBall;

        _setBoolOnStartCutScene -= _paddleBallShooter.DisableShoot;
        _setBoolOnStartCutScene -= _paddleMovement.DisblePaddleMovement;
        _setBoolOnStartCutScene -= _ballDirectionArrow.DisableArrow;
        _setBoolOnStartCutScene -= _paddleVacoom.DisableVacoom;

        _setBoolOnEndCutScene -= _paddleBallShooter.DisableShoot;
        _setBoolOnEndCutScene -= _paddleMovement.DisblePaddleMovement;
        _setBoolOnEndCutScene -= _ballDirectionArrow.DisableArrow;
        _setBoolOnEndCutScene -= _paddleVacoom.DisableVacoom;

        _onEndBossCutScene -= _brickGenerator.StartFirstWaveOfEpisode;

    }
    public void StartCutScene()
    {
        SetUpEvent();
        _setBoolOnStartCutScene?.Invoke(true);
        _onStartCutScene?.Invoke();
    }
    public void EventEnded()
    {
        _cutSceneIndex++;
        if(_cutSceneIndex < _cutSceneEvent.Count)
        {
            SetUpEvent();
        }
        else
        {
            if (_isStartOfBossFight)
            {
                //Start boss fight
                _cutSceneIndex = 0;
                _setBoolOnEndCutScene?.Invoke(false);
                BaseBossBrick bb = _currentBossObject.GetComponent<BaseBossBrick>();
                _episodeTitleCardUI.SetBossIntroText(bb._bossIntroText);
                bb.onStartBossFight?.Invoke();
                _isStartOfBossFight = false;
                
            }
            else
            {
                //End of boss fight
                ResetBoosFightCondition();
                _setBoolOnEndCutScene?.Invoke(false);
                _onEndBossCutScene?.Invoke();
                _timeManager?._onEndBossDay.Invoke();
            }

        }
    }
    private void ResetBoosFightCondition()
    {
        _cutSceneIndex = 0;
        _isStartOfBossFight = true;
        _cutSceneEvent.Clear();
    }
    public void SetUpEvent()
    {
        var content = _cutSceneEvent[_cutSceneIndex];
        var type = content.type;

        _activeCutsceneEventCount = GetActiveEventCount(type);

        if ((type & CUTSCENETYPE.TEXT) != 0)
        {
            _dialougeEvent.SetCutSceneManager(this);
            _dialougeEvent.SetUpContent(content);
            _dialougeEvent.OnEventFinished = HandleCutsceneEventFinished;
            _dialougeEvent.ExecuteEvent();
        }

        if ((type & CUTSCENETYPE.POPIN) != 0)
        {
            _popInCutSceneEvent.SetCutSceneManager(this);
            _popInCutSceneEvent.SetTarget(_currentBossObject);
            _popInCutSceneEvent.SetUpContent(content);
            _popInCutSceneEvent.OnEventFinished = HandleCutsceneEventFinished;
            _popInCutSceneEvent.ExecuteEvent();
        }

        if ((type & CUTSCENETYPE.PARTICLE_EFFECT) != 0)
        {
            GameObject effect = Instantiate(content._particleEffectPrefab, transform.position, Quaternion.identity);
            effect.SetActive(false);

            _particleEffectCutsceneEvent.SetParticleSystem(effect);
            _particleEffectCutsceneEvent.SetUpContent(content);
            _particleEffectCutsceneEvent.OnEventFinished = HandleCutsceneEventFinished;
            _particleEffectCutsceneEvent.ExecuteEvent();
        }

        if ((type & CUTSCENETYPE.CAMSHAKE) != 0)
        {
            _camshakeCutSceneEvent.SetUpContent(content);
            _camshakeCutSceneEvent.OnEventFinished = HandleCutsceneEventFinished;
            _camshakeCutSceneEvent.ExecuteEvent();
        }

        if ((type & CUTSCENETYPE.DESTROY_BOSS) != 0)
        {
            _destryoBossCutSceneEvent.SetUpContent(content);
            _destryoBossCutSceneEvent.SetTarget(_currentBossObject);
            _destryoBossCutSceneEvent.OnEventFinished = HandleCutsceneEventFinished;
            _destryoBossCutSceneEvent.ExecuteEvent();
        }

        if (_activeCutsceneEventCount == 0)
        {
            EventEnded();
        }
    }
    public void SetCurrentBossGameObject(GameObject boss) => _currentBossObject = boss;
    public void FillCutSceneEvent(List<SO_CutSceneEventContent> content) => _cutSceneEvent = content;

    private void HandleCutsceneEventFinished()
    {
        _activeCutsceneEventCount--;

        if (_activeCutsceneEventCount <= 0)
        {
            EventEnded();
        }
    }
    int GetActiveEventCount(CUTSCENETYPE type)
    {
        int count = 0;

        if ((type & CUTSCENETYPE.TEXT) != 0) count++;
        if ((type & CUTSCENETYPE.POPIN) != 0) count++;
        if ((type & CUTSCENETYPE.ANIMATION) != 0) count++;
        if ((type & CUTSCENETYPE.PARTICLE_EFFECT) != 0) count++;
        if ((type & CUTSCENETYPE.CAMSHAKE) != 0) count++;
        if ((type & CUTSCENETYPE.DESTROY_BOSS) != 0) count++;

        return count;
    }
}
