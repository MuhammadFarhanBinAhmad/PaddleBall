using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[System.Flags]
public enum STATUSTYPE
{
    NONE = 0,
    EXPLOSION = 1 << 0,
    DISCHARGE = 1 << 1,
    CRIT = 1 << 2,
    TOXIC = 1 << 3,
    CHANCE = 1 << 4,
    TIMED = 1 << 5,
    ELEMENTAL = 1 << 6
}
public enum DeathCause
{
    NONE,
    NORMAL,
    PADDLE,
    TOWER
}
[System.Serializable]
public class StatusInstance
{
    public ABSAbility _ability;

    public STATUSTYPE type;

    public int stacks;
    public int stackToAdd;
    public int maxStacks;

    public int damagePerStack;

    public GameObject buildupVFX;
    public GameObject activeVFX;

    public float remainingStackTime,stackLifeTime;
    public float remainingEffectTime, timeBeforeEffect;

    public bool resetStackLifeTimeUponHit;
    public GameObject spawnPrefab;

    public bool affectsSpeed;
    public float speedMultiplier;
}
public class BrickBar : MonoBehaviour
{
    BrickUI _brickUI;
    Dictionary<STATUSTYPE, StatusInstance> _statuses = new Dictionary<STATUSTYPE, StatusInstance>();
    List<STATUSTYPE> toRemove = new List<STATUSTYPE>();

    List<BrickModifierBase> _modifiers = new List<BrickModifierBase>();
    List <GameObject> _abilityEffects = new List<GameObject>();

    public List<Transform> _waypoints = new List<Transform>();

    public SO_BrickHealthStats []_brickHealthStats = new SO_BrickHealthStats[5];
    
    BrickPool _brickPool;
    EssencePool _essencePool;
    HitBrickDeathPool _hitBrickDeathPool;

    BrickGenerator _brickGenerator;
    TowerManager _towerManager;
    AbilityManager abilityManager;

    internal BrickHealthComponent _brickHealthComponent;

    [Header("BrickStats")]
    [SerializeField] internal int _elementID;
    [SerializeField] internal int _layerNumber;
    [SerializeField] internal float _tickTimer;
    [SerializeField] internal float _baseFallSpeed;
    [SerializeField] internal float _fallSpeed;
    [SerializeField] internal int _baseDamage;
    [SerializeField] internal SplineContainer _brickPath;
    float progress;
    bool _speedDirty;


    [Header("BrickDamageEffect")]
    [SerializeField] SOLerpAnimationEffect _onDamageAnimEffect;
    [SerializeField] SOLerpAnimationEffect _onDeathAnimEffect;
    [SerializeField] SO_FeedbackEffect so_OnBrickDestroy;
    [SerializeField] ParticleSystem _damageParticle;
    AnimationCurveEffect _AnimCurveEffect;


    [Header("Essence")]
    public int _essenceMinAmountToSpawn;
    public int _essenceMaxAmountToSpawn;



    private void Awake()
    {
        _brickGenerator = FindAnyObjectByType<BrickGenerator>();
        abilityManager = FindAnyObjectByType<AbilityManager>();
        _towerManager = FindAnyObjectByType<TowerManager>();


        _brickPool = FindAnyObjectByType<BrickPool>();
        _essencePool = FindAnyObjectByType<EssencePool>();

        _brickUI = GetComponent<BrickUI>();
        _brickHealthComponent = GetComponent<BrickHealthComponent>();

        _AnimCurveEffect = GetComponent<AnimationCurveEffect>();

        _hitBrickDeathPool = FindAnyObjectByType<HitBrickDeathPool>();

        _brickHealthComponent._onDeath += HandleDeath;
        _brickHealthComponent._onDeath += SpawnEssence;
        _brickHealthComponent._onDeath += _brickGenerator.OnBrickDestroyed;
        _brickHealthComponent._onDeath += _statuses.Clear;
        _brickHealthComponent._onDeath += RemoveAllModifiers;


        _brickHealthComponent._onDeathByPaddle += HandleDeathByPaddle;
        _brickHealthComponent._onDeathByPaddle += _statuses.Clear;
        _brickHealthComponent._onDeathByPaddle += RemoveAllModifiers;
        _brickHealthComponent._onDeathByPaddle += _brickGenerator.OnBrickDestroyed;

        _brickHealthComponent._onDeathByTower += HandleDeathByPaddle;
        _brickHealthComponent._onDeathByTower += _statuses.Clear;
        _brickHealthComponent._onDeathByTower += RemoveAllModifiers;
        _brickHealthComponent._onDeathByTower += _brickGenerator.OnBrickDestroyed;

        _brickHealthComponent.SetBrickBar(this);
    }
    private void OnDestroy()
    {
        _brickHealthComponent._onDeath -= HandleDeath;
        _brickHealthComponent._onDeath -= SpawnEssence;
        _brickHealthComponent._onDeath -= _brickGenerator.OnBrickDestroyed;
        _brickHealthComponent._onDeath -= _statuses.Clear;
        _brickHealthComponent._onDeath -= RemoveAllModifiers;

        _brickHealthComponent._onDeathByPaddle -= HandleDeathByPaddle;
        _brickHealthComponent._onDeathByPaddle -= _statuses.Clear;
        _brickHealthComponent._onDeathByPaddle -= RemoveAllModifiers;
        _brickHealthComponent._onDeathByPaddle -= _brickGenerator.OnBrickDestroyed;

        _brickHealthComponent._onDeathByTower -= HandleDeathByPaddle;
        _brickHealthComponent._onDeathByTower -= _statuses.Clear;
        _brickHealthComponent._onDeathByTower -= RemoveAllModifiers;
        _brickHealthComponent._onDeathByTower -= _brickGenerator.OnBrickDestroyed;

    }
    private void Update()
    {
        float dt = Time.deltaTime;
       

        TickModifiers(dt);

        HandleMovement();

    }
    public void OnDamage(int dmg,DeathCause deathcause = DeathCause.NORMAL)
    {
    }

    public void OnDamageLayer(int amount, DeathCause deathcause = DeathCause.NORMAL)
    {
        //Need resolve branching brick issue on this one
        if (_elementID > 0)
        {
            SO_BrickHealthStats so = null;
            for(int i=0; i < _brickHealthStats.Count(); i++)
            {
                if (_brickHealthStats[i]._elementID == _elementID)
                {
                    so = _brickHealthStats[i];
                }
            }
            _elementID = so._parentElementID;
            SetBrick(_brickHealthStats[_elementID]);
        }
        else
        {
            _brickHealthComponent.PendingDeath(deathcause, true);
        }
    }

    public void UpdateBrickAfterDamage(DeathCause deathcause = DeathCause.NORMAL)
    {
        switch (deathcause)
        {
            case DeathCause.NORMAL:
                {
                    if (_brickHealthComponent.GetHealth() <= 0)
                    {
                        _AnimCurveEffect.PlayEffect(_onDeathAnimEffect, this.gameObject);
                        if (_elementID > 0)
                        {
                            _elementID = _brickHealthStats[_elementID]._parentElementID;
                            SetBrick(_brickHealthStats[_elementID]);
                        }
                        else
                        {
                            _brickHealthComponent.PendingDeath(deathcause, true);
                        }
                    }
                    else
                    {
                        _AnimCurveEffect.PlayEffect(_onDamageAnimEffect, this.gameObject);
                        _damageParticle.Play();
                    }
                    break;
                }

        }
        
        _brickUI.UpdateHealth(_brickHealthComponent.GetStartingHealth(), _brickHealthComponent.GetHealth());
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_brickHit, transform.position);
    }
    public void HandleInstantKill(DeathCause deathcause = DeathCause.NORMAL)
    {
        _brickHealthComponent.PendingDeath(deathcause, true);
    }

    void HandleDeath()
    {
        GlobalFeedbackManager.Instance.SetFeedbackValue(so_OnBrickDestroy);
        GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
        GlobalFeedbackManager.Instance.PlayFreezeFrame();
        abilityManager.NotifyBrickDestroyed(this);
        _brickPool.RemoveActiveBrick(this.gameObject);
        GameObject _vfx = _hitBrickDeathPool.GetObject();
        _vfx.transform.position = transform.position;
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_brickDestroy, transform.position);

        ResetToDefault();

        gameObject.SetActive(false);
    }
    void HandleDeathByPaddle()
    {
        abilityManager.NotifyBrickDestroyed(this);
        _brickPool.RemoveActiveBrick(this.gameObject);
        GameObject _vfx = _hitBrickDeathPool.GetObject();
        _vfx.transform.position = transform.position;
        AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_brickDestroy, transform.position);
        
        gameObject.SetActive(false);
    }
    void SpawnEssence()
    {
        int essencetoSpawn = UnityEngine.Random.Range(_essenceMinAmountToSpawn, _essenceMaxAmountToSpawn);
        for (int i = 0; i < essencetoSpawn; i++)
        {
            GameObject essence = _essencePool.GetEssence();
            essence.transform.position = transform.position;
        }
    }

    public void ApplyStatus(AbilityContext _statusEffect)
    {
        //check if status already exist
        if (_statuses.TryGetValue(_statusEffect._statusType, out StatusInstance existing))
        {

            if (existing.stacks >= (int)_statusEffect._Stats[STATID.MAX_STACKS])
                existing.stacks = (int)_statusEffect._Stats[STATID.MAX_STACKS];
            else
                existing.stacks += (int)_statusEffect._Stats[STATID.STACKS_TO_ADD];

            if (existing.resetStackLifeTimeUponHit)
            {
                existing.remainingStackTime = existing.stackLifeTime;
            }

            if (existing.affectsSpeed)
                MarkSpeedDirty();

            return;
        }
        else
        {
            StatusInstance sinst = new StatusInstance
            {
                _ability = _statusEffect._abililty,
                type = _statusEffect._statusType,
                stacks = (int)_statusEffect._Stats[STATID.STACKS_TO_ADD],
                maxStacks = (int)_statusEffect._Stats[STATID.MAX_STACKS],
                damagePerStack = (int)_statusEffect._Stats[STATID.DAMAGE_PER_STACK],
                stackLifeTime = (int)_statusEffect._Stats[STATID.STACK_LIFETIME],
                remainingStackTime = (int)_statusEffect._Stats[STATID.STACK_LIFETIME],
                timeBeforeEffect = (int)_statusEffect._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE],
                remainingEffectTime = (int)_statusEffect._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE],
                resetStackLifeTimeUponHit = _statusEffect._Statsbool[STATID.RESET_STACK_TIMER],
                spawnPrefab = _statusEffect._spawnPrefab,
                affectsSpeed = _statusEffect._Statsbool[STATID.AFFECTS_SPEED],
                speedMultiplier = _statusEffect._Stats[STATID.SPEED_MULTIPLIER]
                
            };


            _statuses.Add(_statusEffect._statusType, sinst);
            if (sinst.affectsSpeed)
                MarkSpeedDirty();
        }
    }
    public void SetBrick(SO_BrickHealthStats _stats)
    {
        _baseFallSpeed = _stats._dropSpeed;
        _fallSpeed = _stats._dropSpeed;
        _elementID = _stats._elementID;
        _layerNumber = _stats._layerNumber;
        _brickHealthComponent.SetHealth(_stats._health);
        _brickUI.PrepBrickLayerColour(_stats._layerNumber);
        _brickUI.UpdateHealth(_brickHealthComponent.GetStartingHealth(), _brickHealthComponent.GetHealth());
    }
    public void SetBrickPath(SplineContainer _path)
    {
        _brickPath = _path;
        progress = 0f;

        transform.position =
            _brickPath.EvaluatePosition(progress);
    }
    void MarkSpeedDirty() => _speedDirty = true;

    void HandleMovement()
    {
        if (_brickPath != null)
        {
            progress += _fallSpeed * Time.deltaTime;

            if (progress > 1f)
                progress = 1f;
            transform.position =
                _brickPath.EvaluatePosition(progress);
        }

    }
    public void RecalculateSpeed(float speedMultiplier = 0)
    {
        _fallSpeed = _baseFallSpeed * (1 - speedMultiplier);
    }
    //MODIFIERS
    public BrickModifierBase AddModifier(SOBrickModifier modifierPrefab)
    {
        if (modifierPrefab == null) return null;
        // instantiate as child of the brick (could come from a pool)
        var instance = Instantiate(modifierPrefab._modifierPrefab, transform.position, Quaternion.identity);
        instance.Initialize(this);
        _modifiers.Add(instance);
        return instance;
    }
    public void RemoveModifier(BrickModifierBase instance)
    {
        if (instance == null) return;
        if (_modifiers.Contains(instance))
        {
            _modifiers.Remove(instance);
            instance.OnRemove();
        }
    }
    public void RemoveAllModifiers()
    {
        // iterate copy to avoid modifying while iterating
        var copy = new List<BrickModifierBase>(_modifiers);
        foreach (var m in copy)
        {
            RemoveModifier(m);
        }
    }
    void TickModifiers(float dt)
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            var m = _modifiers[i];
            if (m != null) m.Tick(dt);
        }
    }

    void ResetToDefault()
    {
        _elementID = 0;
        _layerNumber = 0;
        _tickTimer = 0;
        _baseFallSpeed = 0;
        _fallSpeed = 0;
        _baseDamage = 0;
        _brickPath = null;
        progress = 0;
        _speedDirty = false;
        transform.parent = _brickPool.transform;
    }
    public int GetLayer() => _layerNumber;
    public int GetShieldDamageValue () => _layerNumber * _baseDamage;
}
