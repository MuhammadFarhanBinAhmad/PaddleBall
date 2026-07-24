using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class ActiveStatusVFX
{
    public GameObject buildup;
    public GameObject pop;
}
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
    [SerializeField] GameObject _damageText;
    Dictionary<STATUSTYPE, ActiveStatusVFX> _activeVFX =
    new Dictionary<STATUSTYPE, ActiveStatusVFX>();

    bool _vulnerableToDamage = true;

    public Action _onDeathByPaddle;
    public Action _onDeathByTower;
    public Action _onDeath;
    DeathCause pendingDeathCause;
    bool pendingDeath;


    private void Update()
    {
        if (_health > 0)
            ExecuteStatusEffect();

        if(pendingDeath)
        {
            if (transform.CompareTag("Brick"))
            {
                ResolveDeath();
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
                    RemoveStatusVFX(status.type);
                    toRemove.Add(kvp.Key);
                }
                else
                {
                    // Restart decay timer for next stack
                    status.remainingStackTime = status.stackLifeTime;
                }

                status._ability.ActivateAbility(this.gameObject);
            }
            if (status.stacks > 0)
            {
                //Effect timer
                status.remainingEffectTime -= dt;
                if (status.remainingEffectTime <= 0)
                {
                    status.remainingEffectTime = status.timeBeforeEffect;
                    OnDamage(status.stacks * status.damagePerStack); //total stack * stack/dmg
                    PlayPopVFX(status.type);
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
        if(!_vulnerableToDamage)
            return;

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
            GameObject dmgText = Instantiate(_damageText, transform.position, Quaternion.identity);
            dmgText.GetComponent<DamageTextFeedback>().SetValue(modified);
            for (int i = 0; i < _modifiers.Count; i++)
                _modifiers[i]?.OnDamageApplied(modified);

            if (transform.CompareTag("Brick"))
            {
                BrickBar bb = GetComponentInParent<BrickBar>();
                bb.UpdateBrickAfterDamage(deathcause);
            }
            else if (transform.CompareTag("Boss"))
            {
                BaseBossBrick bbb = GetComponentInParent<BaseBossBrick>();
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
    public void SpawnStatusVFX(
        STATUSTYPE type,
        GameObject buildupPrefab,
        GameObject popPrefab)
    {
        if (_activeVFX.ContainsKey(type))
            return;

        ActiveStatusVFX vfx = new ActiveStatusVFX();

        if (buildupPrefab != null)
        {
            vfx.buildup =
                Instantiate(buildupPrefab, transform);
        }
        vfx.pop = popPrefab;
        print(vfx.buildup);
        print(vfx.pop);

        _activeVFX.Add(type, vfx);
    }
    void RemoveStatusVFX(STATUSTYPE type)
    {
        if (!_activeVFX.TryGetValue(type, out var vfx))
            return;


        if (vfx.buildup != null)
        {
            Destroy(vfx.buildup);

        }

        _activeVFX.Remove(type);
    }
    void PlayPopVFX(STATUSTYPE type)
    {
        if (!_activeVFX.TryGetValue(type, out var vfx))
            return;

        if (vfx.pop != null)
        {
            Instantiate(
                vfx.pop,
                transform.position,
                Quaternion.identity);
        }
    }
    public void SetHealth(int value)
    {
        _startingHealth = value;
        _health = _startingHealth;
    }

    public void ResolveDeath()
    {
        switch (pendingDeathCause)
        {
            case DeathCause.NORMAL:
                {
                    _onDeath?.Invoke();
                    break;
                }
            case DeathCause.TOWER:
                {
                    _onDeathByTower?.Invoke();
                    break;
                }
            case DeathCause.PADDLE:
                {
                    _onDeathByPaddle?.Invoke();
                    break;
                }
        }
        RemoveAllStatus();

    }
    void RemoveAllStatus()
    {
        pendingDeathCause = DeathCause.NONE;
        pendingDeath = false;
        foreach (var kvp in _statuses)
        {
            var status = kvp.Value;
            status.stacks = 0;
            RemoveStatusVFX(kvp.Key);
            toRemove.Add(kvp.Key);
        }
        toRemove.Clear();
    }


    public void OnDeathByBrick()
    {
        pendingDeathCause = DeathCause.PADDLE;
        pendingDeath = true;
    }
    public void PendingDeath(DeathCause cause, bool state)
    {
        pendingDeathCause = cause;
        pendingDeath = state;
    }
    public int GetHealth() => _health;
    public int GetStartingHealth() => _startingHealth;
    public void ModifyHealth(int amount)
    {
        _health += amount;
        _health = Mathf.Clamp(_health, 0, _startingHealth);
    }
    public void SetVulnerableToAttack(bool status) => _vulnerableToDamage = status;
}
