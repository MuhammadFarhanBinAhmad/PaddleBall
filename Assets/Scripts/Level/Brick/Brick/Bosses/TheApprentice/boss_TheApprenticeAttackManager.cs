using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

enum APPRENTICECURRENTATTACK
{
    CONJURING,
    MOVE_AND_SHOOTING,
    SPAWNING_SHIELD,
    SPAWNING_COPIES
}


public class boss_TheApprenticeAttackManager : BaseBossAttackManager
{

    PaddleMovement _paddleMovement;
    boss_TheApprenticeManager _TheApprenticeManager;
    [SerializeField]BrickAbilityManager _brickAbilityManager;
    [Header("Point And Movement")]
    [SerializeField]
    private List<SplineContainer> _movementPos = new List<SplineContainer>();
    public Transform _target;
    public Transform _spawnPos;
    private Transform _apprenticeParent;
    public SO_BrickSpecialEffect _lerpInEffect,_lerpOutEffect;

    APPRENTICECURRENTATTACK _currentAttack;

    [Header("Attack One")]
    [SerializeField] ABILITY _attackAbility;
    [SerializeField] SO_BrickAbilityStats _shotgunAttack;
    [Header("Attack Two")]
    //Slow down
    [SerializeField] float _conjuringSpellTime;
    [SerializeField] GameObject _conjuringSpellEffect;
    [SerializeField] GameObject _conjuringSpellEnd;
    [SerializeField] float _slowDownDuration;
    [SerializeField] float _slowDownTimeScale;
    [SerializeField] float _curseDuration;
    Coroutine _conjuringRoutine;
    [Header("Attack Three")]
    [SerializeField] ABILITY _shieldAbility;
    [SerializeField] SO_BrickAbilityStats _shieldAttack;
    [Header("Attack Four")]
    [SerializeField] ABILITY _attackAbility_two;
    [SerializeField] SO_BrickAbilityStats _burstFireAttack;
    [Header("Resting Movement")]
    [SerializeField] private Transform _restPoint;
    [SerializeField] private float _restMovementSpeed = 5f;
    [SerializeField] private float _restArrivalDistance = 0.05f;

    private Coroutine _restMovementRoutine;
    private bool _hasReachedRestPoint;
    //[SerializeField] List<Transform> _telePos;
    //[SerializeField] float _stayDuration;
    //[SerializeField] float _teleportDuration;



    private void Awake()
    {
        _TheApprenticeManager = GetComponentInParent<boss_TheApprenticeManager>();
        _target = FindAnyObjectByType<PaddleBallShooter>().transform;
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _apprenticeParent = transform.parent;
    }

    private void Start()
    {
        _TheApprenticeManager._onTakingDamage += StopConjuringSpell;
    }
    private void OnDestroy()
    {
        _TheApprenticeManager._onTakingDamage -= StopConjuringSpell;

    }
    public override void AttackPatternOne()
    {
        BeginPointMovement();
        _brickAbilityManager.SetBrickAbilityStats(_shotgunAttack);
        _brickAbilityManager.ActivateAbilities(_attackAbility);
        _currentAttack = APPRENTICECURRENTATTACK.MOVE_AND_SHOOTING;
    }

    public override void AttackPatternTwo()
    {
        _conjuringRoutine = StartCoroutine(
            StartConjuringSequence()
        );

    }

    public override void AttackPatternThree()
    {
        BeginPointMovement();
        _brickAbilityManager.SetBrickAbilityStats(_shieldAttack);
        _brickAbilityManager.ActivateAbilities(_shieldAbility);
        _currentAttack = APPRENTICECURRENTATTACK.SPAWNING_SHIELD;
    }

    public override void AttackPatternFour()
    {
        BeginPointMovement();
        _brickAbilityManager.SetBrickAbilityStats(_burstFireAttack);
        _brickAbilityManager.ActivateAbilities(_attackAbility_two);
        _currentAttack = APPRENTICECURRENTATTACK.MOVE_AND_SHOOTING;

    }

    public override void RestToNeutral()
    {
        Debug.Log("Boss is resting / resetting to neutral");
    }
    protected void BeginPointMovement()
    {
        _currentAttackEndMode = AttackEndMode.Point;
        _attackComplete = false;

        if (_movementPos == null || _movementPos.Count == 0)
        {
            Debug.LogWarning($"{name}: No movement splines assigned.");
            CompleteCurrentAttack();
            return;
        }

        StopPingPongMovement();

        _movementRoutine = StartCoroutine(SplineMovementRoutine());
    }
    private IEnumerator SplineMovementRoutine()
    {
        GlobalEffects.Instance.PlayLerpObject(
        gameObject,
        _lerpInEffect);

        yield return new WaitForSeconds(
            _lerpInEffect._lerpDuration + 0.25f);

        SplineContainer selectedSpline =
            _movementPos[Random.Range(0, _movementPos.Count)];

        if (selectedSpline == null)
        {
            CompleteCurrentAttack();
            yield break;
        }

        // ------------------------------------
        // Get spline starting position
        // ------------------------------------

        Vector3 splineStartWorld =
            selectedSpline.transform.TransformPoint(
                selectedSpline.EvaluatePosition(0f));

        // Convert world position into
        // The Apprentice parent's local space
        transform.localPosition =
            _apprenticeParent.InverseTransformPoint(
                splineStartWorld);

        GlobalEffects.Instance.PlayLerpObject(
            gameObject,
            _lerpOutEffect);

        yield return new WaitForSeconds(
            _lerpOutEffect._lerpDuration + 0.25f);

        yield return new WaitForSeconds(0.5f);

        // ------------------------------------
        // Start attack
        // ------------------------------------

        switch (_currentAttack)
        {
            case APPRENTICECURRENTATTACK.MOVE_AND_SHOOTING:
                break;

            case APPRENTICECURRENTATTACK.CONJURING:
                StartCoroutine(ConjuringSpell());
                break;
        }

        // ------------------------------------
        // Calculate spline length
        // ------------------------------------

        float splineLength =
            selectedSpline.CalculateLength();

        if (splineLength <= 0f)
        {

            CompleteCurrentAttack();
            yield break;
        }

        float t = 0f;

        // ------------------------------------
        // Move along spline
        // ------------------------------------

        while (t < 1f)
        {
            t +=
                (_movementSpeed / splineLength)
                * Time.deltaTime;

            t = Mathf.Clamp01(t);

            // Spline local ¨ spline world
            Vector3 splineWorldPosition =
                selectedSpline.transform.TransformPoint(
                    selectedSpline.EvaluatePosition(t));

            // Spline world ¨ Apprentice local
            Vector3 parentLocalPosition =
                _apprenticeParent.InverseTransformPoint(
                    splineWorldPosition);

            // Move Apprentice Manager
            transform.localPosition =
                parentLocalPosition;

            yield return null;
        }

        // ------------------------------------
        // Ensure exact final position
        // ------------------------------------

        if (t >= 1f &&
            !_isStunned &&
            !_stopAttacking)
        {
            Vector3 finalWorldPosition =
                selectedSpline.transform.TransformPoint(
                    selectedSpline.EvaluatePosition(1f));

            transform.localPosition =
                _apprenticeParent.InverseTransformPoint(
                    finalWorldPosition);

            CompleteCurrentAttack();

            _brickAbilityManager.ResetAllAbility();
        }

        _movementRoutine = null;
    }
    private IEnumerator StartConjuringSequence()
    {
        // Teleport before starting the attack.
        yield return StartCoroutine(TeleportToSpawn());

        if (_stopAttacking || _isStunned)
            yield break;

        // Start conjuring only after teleporting.
        yield return StartCoroutine(ConjuringSpell());

        _conjuringRoutine = null;
    }
    IEnumerator ConjuringSpell()
    {

        _conjuringSpellEffect.SetActive(true);

        yield return new WaitForSeconds(_conjuringSpellTime);

        int index = Random.Range(0, 1);

        switch (index)
        {
            case 0:
                _paddleMovement.IsCursed(_curseDuration);
                break;
        }

        _conjuringSpellEffect.SetActive(false);
        _conjuringSpellEnd.SetActive(true);

        yield return new WaitForSeconds(_conjuringSpellTime);


        CompleteCurrentAttack();
    }
    public void StopConjuringSpell()
    {
        if (_conjuringRoutine == null)
            return;

        StopCoroutine(_conjuringRoutine);
        _conjuringRoutine = null;

        _conjuringSpellEffect.SetActive(false);

        CompleteCurrentAttack();

        Debug.Log("Conjuring interrupted!");
    }
    public override void StopBossAttack()
    {
        base.StopBossAttack();

        if (_restMovementRoutine != null)
        {
            StopCoroutine(_restMovementRoutine);
            _restMovementRoutine = null;
        }

        _brickAbilityManager.ResetAllAbility();
    }
    public override void Resting()
    {
        if (_restPoint == null)
        {
            Debug.LogWarning("Apprentice rest point not assigned!");
            _hasReachedRestPoint = true;
            return;
        }

        if (_restMovementRoutine != null)
            StopCoroutine(_restMovementRoutine);

        _hasReachedRestPoint = false;

        _brickAbilityManager.ResetAllAbility();

        _restMovementRoutine = StartCoroutine(
            MoveToRestPoint()
        );
    }
    private IEnumerator MoveToRestPoint()
    {
        _hasReachedRestPoint = false;

        // Disable all active abilities before resting.
        _brickAbilityManager.ResetAllAbility();

        // Step 1: Lerp in (disappear).
        GlobalEffects.Instance.PlayLerpObject(
            gameObject,
            _lerpInEffect
        );

        yield return new WaitForSeconds(
            _lerpInEffect._lerpDuration + 0.25f
        );

        if (_stopAttacking || _isStunned)
            yield break;

        // Step 2: Teleport to spawn position.
        transform.position = _spawnPos.position;

        // Step 3: Lerp out (reappear).
        GlobalEffects.Instance.PlayLerpObject(
            gameObject,
            _lerpOutEffect
        );

        yield return new WaitForSeconds(
            _lerpOutEffect._lerpDuration + 0.25f
        );

        if (_stopAttacking || _isStunned)
            yield break;

        // Step 4: Notify the base manager that the boss
        // has arrived at its resting position.
        _hasReachedRestPoint = true;
        _restMovementRoutine = null;
    }
    private IEnumerator TeleportToSpawn()
    {
        if (_spawnPos == null)
            yield break;

        // Step 1: Lerp in (disappear).
        GlobalEffects.Instance.PlayLerpObject(
            gameObject,
            _lerpInEffect
        );

        yield return new WaitForSeconds(
            _lerpInEffect._lerpDuration + 0.25f
        );

        if (_stopAttacking || _isStunned)
            yield break;

        // Step 2: Teleport to spawn position.
        transform.position = _spawnPos.position;

        // Step 3: Lerp out (reappear).
        GlobalEffects.Instance.PlayLerpObject(
            gameObject,
            _lerpOutEffect
        );

        yield return new WaitForSeconds(
            _lerpOutEffect._lerpDuration + 0.25f
        );
    }
    protected override bool IsBossAtRestPoint()
    {
        return _hasReachedRestPoint;
    }
}
