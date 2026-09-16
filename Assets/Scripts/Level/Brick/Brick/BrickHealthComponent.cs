using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
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

    BrickBar _brickBar;

    Dictionary<STATUSTYPE, StatusInstance> _statuses = new Dictionary<STATUSTYPE, StatusInstance>();
    List<STATUSTYPE> toRemove = new List<STATUSTYPE>();
    List<BrickModifierBase> _modifiers = new List<BrickModifierBase>();
    List<GameObject> _abilityEffects = new List<GameObject>();

    [Header("BrickStats")]
    [SerializeField] int _startingHealth;
    [SerializeField] int _health;
    [SerializeField] float _tickTimer;
    [SerializeField] GameObject _damageText;
    [SerializeField] GameObject _hitFlash;
    [SerializeField] float _flashPeriod;
    Dictionary<STATUSTYPE, ActiveStatusVFX> _activeVFX =
    new Dictionary<STATUSTYPE, ActiveStatusVFX>();
    //STATUSTYPE _specialStatus;

    [SerializeField] private float _rayDistance = 10f;
    [SerializeField] private LayerMask _brickLayer;
    [SerializeField] List<BrickHealthComponent> _nearbyBricks = new List<BrickHealthComponent>();

    [Header("DischargeEffect")]
    public GameObject _dischargeBuildVFX;
    public GameObject _dischargePopVFX;
    [Header("ToxicEffect")]
    public GameObject _toxicBuildVFX;
    public GameObject _toxicPopVFX;

    float _dmgMultiplier;

    bool _vulnerableToDamage = true;

    public Action _onDeathByPaddle;
    public Action _onDeathByTower;
    public Action _onDeath;
    DeathCause pendingDeathCause;
    bool pendingDeath;

    internal void SetBrickBar(BrickBar _bb)
    {
        _brickBar = _bb;
    }
    private void Update()
    {
        if (_health > 0)
            ExecuteStatusEffect();

        if (pendingDeath)
        {
            if (transform.CompareTag("Brick"))
            {
                ResolveDeath();
            }
        }
        //FindNearbyBricks();
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
                SetSpeedMultiplier();

                if (status.stacks <= 0)
                {
                    RemoveStatusVFX(status.type);
                    toRemove.Add(kvp.Key);
                }
                else
                {
                    status.remainingStackTime = status.stackLifeTime;
                }

                if (status._ability != null)
                {
                    status._ability.ActivateAbility(gameObject);
                }
            }
            if (status.stacks > 0)
            {
                //Effect timer
                status.remainingEffectTime -= dt;
                if (status.remainingEffectTime <= 0)
                {
                    status.remainingEffectTime = status.timeBeforeEffect;
                    OnDamage(status.stacks * status.damagePerStack, status.type); //total stack * stack/dmg
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
    public void SetSpeedMultiplier()
    {
        float speedMultiplier = 0;

        foreach (var kvp in _statuses)
        {
            var status = kvp.Value;

            if (!status.affectsSpeed)
                continue;

            for (int i = status.stacks; i > 0; i--)
            {
                speedMultiplier += status.speedMultiplier;
            }
        }

        if (_brickBar != null)
        {
            _brickBar.RecalculateSpeed(speedMultiplier);

        }
    }
    public void OnDamage(int dmg, STATUSTYPE dmgType = STATUSTYPE.NONE, DeathCause deathcause = DeathCause.NORMAL, bool isInstantKill = false)
    {
        if (!_vulnerableToDamage)
            return;

        if (!isInstantKill)
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

            float extradmg = modified * _dmgMultiplier;
            modified += (int)extradmg;

            _health -= modified;
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

            SpawnDamageText(modified, dmgType);
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
    public void OnDestroyLayer(int layer = 0)
    {
        for (int i = 0; i < layer; i++)
        {
            OnDamage(_health, STATUSTYPE.CRIT);
        }
    }
    public void OnInstantKill(int threshold)
    {
        BrickBar bb = GetComponent<BrickBar>();

        if (threshold == 0)
        {
            ClearAllStatusEffects();
            //ResetSpecialStatus();
            ResetDamageMultiplier();
            ClearAllNearbyBrickList();

            pendingDeath = false;
            pendingDeathCause = DeathCause.NONE;

            _vulnerableToDamage = true;

            if (_hitFlash != null)
                _hitFlash.SetActive(false);

            SetSpeedMultiplier();

            bb.HandleInstantKill(DeathCause.NORMAL);
            return;
        }
        else if (bb._elementID <= threshold)
        {
            ClearAllStatusEffects();
            //ResetSpecialStatus();
            ResetDamageMultiplier();
            ClearAllNearbyBrickList();

            pendingDeath = false;
            pendingDeathCause = DeathCause.NONE;

            _vulnerableToDamage = true;

            if (_hitFlash != null)
                _hitFlash.SetActive(false);

            SetSpeedMultiplier();

            OnDamage(_health, STATUSTYPE.CRIT);
            bb.HandleInstantKill(DeathCause.NORMAL);
            return;
        }

    }
    public void ApplyStatus(AbilityContext _statusEffect, STATUSTYPE type)
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
            SetSpeedMultiplier();
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
                timeBeforeEffect = _statusEffect._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE],
                remainingEffectTime = _statusEffect._Stats[STATID.TIME_BEFORE_EFFECT_ACTIVATE],
                resetStackLifeTimeUponHit = _statusEffect._Statsbool[STATID.RESET_STACK_TIMER],
                spawnPrefab = _statusEffect._spawnPrefab,
                affectsSpeed = _statusEffect._Statsbool[STATID.AFFECTS_SPEED],
                speedMultiplier = _statusEffect._Stats[STATID.SPEED_MULTIPLIER]
            };
            _statuses.Add(_statusEffect._statusType, sinst);
            SpawnStatusVFX(_statusEffect._statusType);
            SetSpeedMultiplier();
        }
    }
    public void SpawnStatusVFX(
        STATUSTYPE type)
    {
        if (_activeVFX.ContainsKey(type))
            return;
        ActiveStatusVFX vfx = new ActiveStatusVFX();

        GameObject buildupPrefab = null;
        GameObject popPrefab = null;

        switch (type)
        {
            case STATUSTYPE.DISCHARGE:
                {
                    buildupPrefab = _dischargeBuildVFX;
                    popPrefab = _dischargePopVFX;
                    break;
                }
            case STATUSTYPE.TOXIC:
                {
                    buildupPrefab = _toxicBuildVFX;
                    popPrefab = _toxicPopVFX;
                    break;
                }
        }
        if (buildupPrefab != null)
        {
            vfx.buildup =
                Instantiate(buildupPrefab, transform);
            if (popPrefab != null)
            {
                vfx.pop = popPrefab;
            }
            _activeVFX.Add(type, vfx);
        }

    }
    public void RemoveStatusVFX(STATUSTYPE type)
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
        //ResetSpecialStatus();
        ResetDamageMultiplier();
        ClearAllNearbyBrickList();
        _hitFlash.SetActive(false);

    }
    void RemoveAllStatus()
    {
        ClearAllStatusEffects();
        pendingDeathCause = DeathCause.NONE;
        pendingDeath = false;
    }
    public void ClearAllStatusEffects()
    {
        foreach (var kvp in _statuses)
        {
            StatusInstance status = kvp.Value;

            status.stacks = 0;
            status.remainingStackTime = 0f;
            status.remainingEffectTime = 0f;

            RemoveStatusVFX(status.type);
        }

        _statuses.Clear();

        SetSpeedMultiplier();
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
    public void SpawnDamageText(int dmg, STATUSTYPE type)
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 3f;

        Vector3 spawnPosition = transform.position + new Vector3(
            randomOffset.x,
            randomOffset.y,
            0f
        );

        GameObject dmgText = Instantiate(
            _damageText,
            spawnPosition,
            Quaternion.identity
        );

        dmgText.GetComponent<DamageTextFeedback>().SetValue(dmg, type);

        StartCoroutine(HitFlash());
    }
    IEnumerator HitFlash()
    {
        _hitFlash.SetActive(true);
        yield return new WaitForSeconds(_flashPeriod);
        _hitFlash.SetActive(false);
    }
    public void FindNearbyBricks()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                transform.position,
                direction,
                _rayDistance,
                _brickLayer
            );

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null)
                    continue;

                BrickHealthComponent brick =
                    hit.collider.GetComponent<BrickHealthComponent>();

                if (brick == null)
                    continue;

                // Ignore myself, but keep checking further along this ray
                if (brick == this)
                    continue;

                if (!_nearbyBricks.Contains(brick))
                {
                    _nearbyBricks.Add(brick);
                }

                // If you only want the FIRST OTHER brick,
                // stop checking this ray after finding it.
                break;
            }
        }
    }
    public List<BrickHealthComponent> GetAllNearbyBrick() => _nearbyBricks;
    public void ClearAllNearbyBrickList() => _nearbyBricks.Clear();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Gizmos.DrawRay(
                transform.position,
                direction * _rayDistance
            );
        }
    }
    public void ResetDamageMultiplier() => _dmgMultiplier = 0;
    public void ModifyDamageMultiplier(float val) => _dmgMultiplier += val;
    public void SetVulnerableToAttack(bool status) => _vulnerableToDamage = status;
    public StatusInstance GetStatusInstance(STATUSTYPE type) => _statuses[type];

    public bool HasStatus(STATUSTYPE status)
    {
        return _statuses.ContainsKey(status);
    }
    public void AddStatus(STATUSTYPE status)
    {
        if (!_statuses.ContainsKey(status))
        {
            _statuses.Add(status, new StatusInstance());
        }
    }
    public int GetStatusStack(STATUSTYPE status)
    {
        if (_statuses.ContainsKey(status))
            return _statuses[status].stacks;

        return 0;
    }
    public int GetMaxStack(STATUSTYPE status)
    {
        if (_statuses.ContainsKey(status))
            return _statuses[status].maxStacks;

        return 0;
    }
}
