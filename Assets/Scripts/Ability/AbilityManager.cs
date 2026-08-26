using System.Collections.Generic;
using UnityEngine;



public class AbilityManager : MonoBehaviour
{
    // now store abilities directly as ABSAbility instances
    public List<ABSAbility> _brickAbilities = new List<ABSAbility>();
    public List<ABSAbility> _ballAbilities = new List<ABSAbility>();

    Dictionary<string, ABSAbility> _abilitiesByID = new Dictionary<string, ABSAbility>();

    public List<SOAbilityEffect> test = new List<SOAbilityEffect>();
    private void Start()
    {
        foreach (var abilitySo in test)
        {
            AddAbility(abilitySo);
        }
    }

    /// <summary>
    /// Add ability from SO. Returns the created ABSAbility instance, or null on failure.
    /// </summary>
    public ABSAbility AddAbility(SOAbilityEffect so)
    {
        if (so == null || so._abilityPrefab == null)
        {
            Debug.LogError("Ability SO or prefab missing.");
            return null;
        }

        GameObject go = Instantiate(so._abilityPrefab, transform);
        ABSAbility ability = go.GetComponent<ABSAbility>();

        if (ability == null)
        {
            Debug.LogError("Prefab does not contain ABSAbility.");
            Destroy(go);
            return null;
        }

        ability._SOAbilityEffect = so;
        ability.OnAdded(this);

        // store ability directly
        _brickAbilities.Add(ability);

        string id = so._abilityName;

        if (!_abilitiesByID.ContainsKey(id))
        {
            _abilitiesByID.Add(id, ability);
        }
        else
        {
            Debug.LogWarning($"Ability {id} already exists.");
        }
        UnlockAbility(id);

        return ability;
    }

    /// <summary>
    /// Remove ability instance. Returns the removed ABSAbility (so caller can inspect), or null if not found.
    /// </summary>
    public ABSAbility RemoveAbility(ABSAbility ability)
    {
        if (ability == null)
            return null;

        if (_brickAbilities.Remove(ability))
        {
            string id = ability._SOAbilityEffect._abilityName;

            if (_abilitiesByID.ContainsKey(id))
                _abilitiesByID.Remove(id);

            Destroy(ability.gameObject);
            return ability;
        }

        return null;
    }

    public bool HasAbility(string id)
    {
        return _abilitiesByID.ContainsKey(id);
    }

    public T GetAbility<T>(string id) where T : ABSAbility
    {
        if (_abilitiesByID.TryGetValue(id, out ABSAbility ability))
            return ability as T;

        return null;
    }

    public bool IsAbilityUnlocked(string id)
    {
        if (_abilitiesByID.TryGetValue(id, out ABSAbility ability))
            return ability.IsUnlocked;

        return false;
    }
    public void UnlockAbility(string id)
    {
        if (_abilitiesByID.TryGetValue(id, out ABSAbility ability))
        {
            ability.SetUnlocked(true);
        }
    }

    // „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ Brick Events „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ

    public void NotifyBrickHit(BrickHealthComponent health, int basedmg)
    {
        //create basic hit context
        HitContext ctx = new HitContext
        {
            _brick = null,
            _health = health,
            _damageValue = basedmg,
        };

        // Phase 1: Modifier
        //foreach (var ability in _brickAbilities)
        //    ability.ModifyHit(ctx);

        // Phase 2: On hit(Add->Subtract->Multiply->Division)
        foreach (var ability in _brickAbilities)
            ability.OnHit(ctx);//For base abilities

        //For modifiying existing ability
        foreach (var ability in _brickAbilities)
            ability.OnHitAdd(ctx);

        foreach (var ability in _brickAbilities)
            ability.OnHitSubtract(ctx);

        foreach (var ability in _brickAbilities)
            ability.OnHitMultiply(ctx);

        foreach (var ability in _brickAbilities)
            ability.OnHitDivide(ctx);

            // Phase 3: Apply damage
            ctx._health.OnDamage(ctx._damageValue);

        // Phase 4: Notify abilities of outcome
        foreach (var ability in _brickAbilities)
            ability.OnHitResolved(ctx);
    }

    public void NotifyBrickDestroyed(BrickBar brick)
    {
        foreach (var ability in _brickAbilities)
        {
            ability.OnBrickDestroy(brick);
        }
    }

    public void NotifyBallDestroyed(Ball ball)
    {
        foreach (var ability in _brickAbilities)
        {
            ability.OnBallDestroy(ball);
        }
    }

    private float _tickTimer;

    private void Update()
    {
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= 1f)
        {
            _tickTimer -= 1f;

            foreach (var ability in _brickAbilities)
            {
                ability.OnTick(1f);
            }
        }
    }

    public ABSAbility GetAbility(string id)
    {
        if (_abilitiesByID.TryGetValue(id, out ABSAbility ability))
            return ability;

        return null;
    }
    public void ApplyDischargeModifiers(HitContext hitCtx, AbilityContext dischargeCtx)
    {
        foreach (var ability in _brickAbilities)
        {
            if (ability is IDischargeContextModifier modifier)
            {
                modifier.ModifyDischargeAdd(hitCtx, dischargeCtx);
                modifier.ModifyDischargeSubtract(hitCtx, dischargeCtx);
                modifier.ModifyDischargeMultiple(hitCtx, dischargeCtx);
                modifier.ModifyDischargeDivide(hitCtx, dischargeCtx);

            }
        }
    }
    public void ApplyExplosionModifiers(HitContext hitCtx,ExplosionContext explosionCtx)
    {
        foreach (var ability in _brickAbilities)
        {
            if (ability is IExplosionContextModifier modifier)
            {
                modifier.ModifyExplosionContextAdd(hitCtx, explosionCtx);
                modifier.ModifyExplosionContextSubtract(hitCtx, explosionCtx);
                modifier.ModifyExplosionContextMultiply(hitCtx, explosionCtx);
                modifier.ModifyExplosionContextDivide(hitCtx, explosionCtx);

            }
        }
    }
    public void ApplyToxicModifiers(AbilityContext dischargeCtx)
    {
        foreach (var ability in _brickAbilities)
        {
            if (ability is IToxicContextModifier modifier)
            {
                modifier.ModifyToxicContextAdd(dischargeCtx);
                modifier.ModifyToxicContextSubtract(dischargeCtx);
                modifier.ModifyToxicContextMultiple(dischargeCtx);
                modifier.ModifyToxicContextDivide(dischargeCtx);

            }
        }
    }
    public void ApplyCriticalModifiers(HitContext hitCtx, AbilityContext CriticalCtx)
    {
        foreach (var ability in _brickAbilities)
        {
            if (ability is ICriticalContextModifier modifier)
            {
                modifier.ModifyCriticalContextAdd(hitCtx, CriticalCtx);
                modifier.ModifyCriticalContextSubtract(hitCtx, CriticalCtx);
                modifier.ModifyCriticalContextMultiply(hitCtx, CriticalCtx);
                modifier.ModifyCriticalContextDivide(hitCtx, CriticalCtx);

            }
        }
    }
    public void ApplyFireModifiers(HitContext hitCtx, ref HotZoneArea hza)
    {
        foreach (var ability in _brickAbilities)
        {
            if (ability is IFireContextModifier modifier)
            {
                modifier.ModifyFireContext(hitCtx, ref hza);
            }
        }
    }
    public bool RemoveAbility(string id)
    {
        if (_abilitiesByID.TryGetValue(id, out ABSAbility ability))
        {
            return RemoveAbility(ability) != null;
        }

        return false;
    }
}
