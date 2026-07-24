using System;
using UnityEngine;

public class boss_TheApprenticeShieldManager : MonoBehaviour
{
    [SerializeField] BaseBossAttackManager _baseBossManager;
    [SerializeField] BrickHealthComponent _brickHealthComponent;

    BoxCollider2D _boxCollider;
    [SerializeField] GameObject _shieldRenderer;

    bool _isShieldUp;

    public float _maxStunTime;
    public float _currentStunTime;

    public Action _onShieldDown;
    public Action _onShieldUp;
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        _onShieldDown += StunBoss;
        _onShieldDown += DeactivateShield;

        _onShieldUp += ResetShield;
    }
    private void OnDestroy()
    {
        _onShieldDown -= StunBoss;
        _onShieldDown -= DeactivateShield;

        _onShieldUp -= ResetShield;

    }
    private void Update()
    {
        if(_isShieldUp)
            return;

        if(_currentStunTime <= _maxStunTime)
            _currentStunTime += Time.deltaTime;
        else
            _onShieldUp?.Invoke();

    }
    public void StunBoss()
    {
        _baseBossManager.StunBoss(_maxStunTime);
    }
    void DeactivateShield()
    {
        _isShieldUp = false;
        _currentStunTime = 0;
        _brickHealthComponent.SetVulnerableToAttack(true);
        _boxCollider.enabled = _isShieldUp;
        _shieldRenderer.SetActive(_isShieldUp);
    }
    void ResetShield()
    {
        _isShieldUp = true;
        _brickHealthComponent.SetVulnerableToAttack(false);
        _boxCollider.enabled = _isShieldUp;
        _shieldRenderer.SetActive(_isShieldUp);
    }

}
