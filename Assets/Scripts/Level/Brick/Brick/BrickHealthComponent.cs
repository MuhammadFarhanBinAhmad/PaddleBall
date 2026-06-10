using System;
using System.Collections.Generic;
using UnityEngine;

public class BrickHealthComponent : MonoBehaviour
{
    EssencePool _essencePool;

    TowerManager _towerManager;
    AbilityManager abilityManager;

    Dictionary<STATUSTYPE, StatusInstance> _statuses = new Dictionary<STATUSTYPE, StatusInstance>();
    List<STATUSTYPE> toRemove = new List<STATUSTYPE>();
    List<BrickModifierBase> _modifiers = new List<BrickModifierBase>();
    List<GameObject> _abilityEffects = new List<GameObject>();

    [Header("BrickStats")]
    [SerializeField] int _startingHealth;
    [SerializeField] int _health;
    [SerializeField] float _tickTimer;

    public Action _onDeath;
    bool pendingDeath;

    [Header("Test")]
    [SerializeField] SO_BossBrickStats _bossBrickStats;


    private void Start()
    {
        SetHealth(_bossBrickStats);
    }
    private void Update()
    {
        if (_health > 0)
            ExecuteStatusEffect();

        if(pendingDeath)
        {
            if (transform.CompareTag("Brick"))
            {
                BrickBar bb = GetComponent<BrickBar>();
                bb.ResolveDeath();
            }
        }
    }

    void ExecuteStatusEffect()
    {
        float dt = Time.deltaTime;
        toRemove.Clear();

        if (pendingDeath)
            return;

        foreach (var kvp in _statuses)
        {
            var status = kvp.Value;
            //Damage effect timer
            //if (status.type == STATUSTYPE.STUN)
            //{
            //    _fallSpeed = 0;
            //}
            //else
            //{
            //    // DOT tick
            //    status.remainingEffectTime -= dt;
            //    if (status.remainingEffectTime <= 0)
            //    {
            //        status.remainingEffectTime = status.timeBeforeEffect;
            //        OnDamage(status.stacks * status.damagePerStack); //total stack * stack/dmg
            //    }
            //}

            // Stack timer
            status.remainingStackTime -= dt;
            if (status.remainingStackTime <= 0f)
            {
                status.stacks--;

                if (status.stacks <= 0)
                {
                    toRemove.Add(kvp.Key);
                }
                else
                {
                    // Restart decay timer for next stack
                    status.remainingStackTime = status.stackLifeTime;
                }

                status._ability.ActivateAbility();
            }
            if (status.stacks > 0)
            {
                //Effect timer
                status.remainingEffectTime -= dt;
                if (status.remainingEffectTime <= 0)
                {
                    status.remainingEffectTime = status.timeBeforeEffect;
                    print("TotalDmg via stack" + status.stacks * status.damagePerStack);
                    OnDamage(status.stacks * status.damagePerStack); //total stack * stack/dmg
                }
            }

        }

        //remove all completed status effect
        foreach (var key in toRemove)
        {
            _statuses.Remove(key);
        }
    }
    public void OnDamage(int dmg, DeathCause deathcause = DeathCause.NORMAL, bool isInstantKill = false)
    {
        if(!isInstantKill)
        {
            if (dmg == 0) dmg = 1;

            int modified = dmg;
            for (int i = 0; i < _modifiers.Count; i++)
            {
                if (_modifiers[i] != null)
                    modified = _modifiers[i].ModifyIncomingDamage(modified);
            }
            //For Hit shield effect
            if (modified <= 0)
                return;

            _health -= modified;
            for (int i = 0; i < _modifiers.Count; i++)
                _modifiers[i]?.OnDamageApplied(modified);

            if (transform.CompareTag("Brick"))
            {
                BrickBar bb = GetComponent<BrickBar>();
                bb.UpdateBrickAfterDamage(deathcause);
            }
            else if (transform.CompareTag("Boss"))
            {
                BaseBossBrick bbb = GetComponent<BaseBossBrick>();
                bbb.HandleDamage(dmg);
            }
        }
        else
        {
            if (transform.CompareTag("Brick"))
            {
                BrickBar bb = GetComponent<BrickBar>();
                bb.HandleInstantKill(deathcause);
            }
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
        }
    }
    public void SetHealth(SO_BossBrickStats _stats)
    {
        _startingHealth = _stats._health;
        _health = _startingHealth;
    }
    public void SetHealth(SO_BrickHealthStats _stats)
    {
        _startingHealth = _stats._health;
        _health = _startingHealth;
    }
    public int GetHealth() => _health;
    public int GetStartingHealth() => _startingHealth;
    public void ModifyHealth(int amount)
    {
        _health += amount;

        _health = Mathf.Clamp(_health, 0, _startingHealth);
    }

}
