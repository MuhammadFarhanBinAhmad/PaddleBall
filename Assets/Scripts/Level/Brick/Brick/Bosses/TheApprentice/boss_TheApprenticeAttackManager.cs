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
    [Header("Point And Movement")]
    [SerializeField]
    private List<SplineContainer> _movementPos = new List<SplineContainer>();
    public Transform _target;
    public Transform _spawnPos;
    private Transform _apprenticeParent;
    public SO_BrickSpecialEffect _lerpInEffect,_lerpOutEffect;

    APPRENTICECURRENTATTACK _currentAttack;

    [Header("Attack One")]
    [SerializeField] GameObject _projectile;
    [SerializeField] GameObject _conjuringProjectileEffect;

    public float _projectileRest;
    public float _projectileBuildUpAttack;
    [Header("Attack Two")]
    //Slow down
    [SerializeField] float _conjuringSpellTime;
    [SerializeField] GameObject _conjuringSpellEffect;
    [SerializeField] GameObject _conjuringSpellEnd;
    [SerializeField] float _slowDownDuration;
    [SerializeField] float _slowDownTimeScale;
    [SerializeField] float _curseDuration;

    [Header("Attack Three")]
    [SerializeField] SO_BrickSpecialEffect _spawnBrickEffect;
    [SerializeField] SO_BrickHealthStats _stats;
    BrickPool _brickPool;
    [SerializeField] List<Transform> _pos = new List<Transform>();
    [SerializeField] List<BrickBar> _active = new List<BrickBar>();
    [SerializeField] float _timeToSpawnBrick;
    Coroutine _conjuringRoutine;
    //[SerializeField] List<Transform> _telePos;
    //[SerializeField] float _stayDuration;
    //[SerializeField] float _teleportDuration;



    private void Awake()
    {
        _TheApprenticeManager = GetComponentInParent<boss_TheApprenticeManager>();
        _target = FindAnyObjectByType<PaddleBallShooter>().transform;
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _brickPool = FindAnyObjectByType<BrickPool>();
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
        _currentAttack = APPRENTICECURRENTATTACK.MOVE_AND_SHOOTING;
    }

    public override void AttackPatternTwo()
    {
        StartSpawningShield();
        _currentAttack = APPRENTICECURRENTATTACK.SPAWNING_SHIELD;
    }

    public override void AttackPatternThree()
    {
        _conjuringRoutine = StartCoroutine(ConjuringSpell());
    }

    public override void RestToNeutral()
    {
        Debug.Log("Boss is resting / resetting to neutral");
    }
    protected void BeginTimedAttack(float duration)
    {
        _currentAttackEndMode = AttackEndMode.Duration;
        _attackEndTime = Time.time + Mathf.Max(0f, duration);
        _attackComplete = false;
    }
    //Attacks
    public void StartSpawningShield()
    {
        StartCoroutine(SpawnBrick());
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
                _conjuringProjectileEffect.SetActive(true);
                StartCoroutine(ShootNormalProjectile());
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
            Debug.LogWarning(
                $"{name}: Selected spline has no length.");

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
        }

        _movementRoutine = null;
    }
    IEnumerator ShootNormalProjectile()
    {

        yield return new WaitForSeconds(_lerpOutEffect._lerpDuration + .25f);

        while (IsAttackActive)
        {
            _conjuringProjectileEffect.SetActive(true);
            yield return new WaitForSeconds(_projectileBuildUpAttack);
            GameObject proj = Instantiate(_projectile, _spawnPos.position, Quaternion.identity);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            ep.ShootProjectile(_target);
            _conjuringProjectileEffect.SetActive(false);
            yield return new WaitForSeconds(_projectileRest);
        }
    }
    IEnumerator ConjuringSpell()
    {

        GlobalEffects.Instance.PlayLerpObject(this.gameObject, _lerpInEffect);
        yield return new WaitForSeconds(_lerpInEffect._lerpDuration + .25f);
        _conjuringSpellEffect.SetActive(true);
        transform.position = _spawnPos.position;
        GlobalEffects.Instance.PlayLerpObject(this.gameObject, _lerpOutEffect);
        yield return new WaitForSeconds(_lerpOutEffect._lerpDuration + 0.5f);
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

        CompleteCurrentAttack();
        _movementRoutine = null;
    }

    IEnumerator SpawnBrick()
    {
        
        for (int i =0; i < _pos.Count;i++)
        {
            if (_active[i] == null)
            {
                yield return new WaitForSeconds(_timeToSpawnBrick);
                GameObject _bb = _brickPool.GetBrick();
                BrickBar brickBar = _bb.GetComponent<BrickBar>();
                brickBar.SetBrick(_stats);
                _bb.transform.position = _pos[i].position;
                _bb.transform.parent = _pos[i].transform;
                GlobalEffects.Instance.PlayLerpObject(_bb, _spawnBrickEffect);
                _active[i] = brickBar;
                continue;
            }
            if (_active[i].gameObject.activeInHierarchy)
            {
                continue;
            }
            else
            {
                yield return new WaitForSeconds(_timeToSpawnBrick);
                _active[i].gameObject.SetActive(true);
                _active[i].SetBrick(_stats);
                _active[i].transform.position = _pos[i].position;
                _active[i].transform.parent = _pos[i].transform;
                GlobalEffects.Instance.PlayLerpObject(_active[i].gameObject, _spawnBrickEffect);
            }
        }

        CompleteCurrentAttack();
        _movementRoutine = null;
    }
    public void StopConjuringSpell()
    {
        if (_conjuringRoutine != null)
        {
            StopCoroutine(_conjuringRoutine);
            StopCoroutine(SplineMovementRoutine());
            _conjuringSpellEffect.SetActive(false);
            _conjuringRoutine = null;

            CompleteCurrentAttack();
            _movementRoutine = null;
            print("hit");
        }
    }
}
