using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DeadZone : MonoBehaviour
{
    TowerManager _towerManager;

    public Action OnShieldDamage;

    public GameObject _deathVFX;

    [Header("Shield")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _maxShieldMana;
    [SerializeField] float _currentShieldMana;
    [SerializeField] float _coolDownPeriod;
    [SerializeField] float _currentCoolDownTime;
    [SerializeField] float _shieldRegenRate;

    Color shieldColour;
    private void Start()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
        shieldColour = _spriteRenderer.color;
        _currentShieldMana = _maxShieldMana;

        UpdateShieldVisual();

    }
    private void Update()
    {
        if (_currentShieldMana >= _maxShieldMana) return;

        if(_currentShieldMana < _maxShieldMana)
        {
            if(_currentCoolDownTime > 0)
                _currentCoolDownTime -= Time.deltaTime;
            else
                _currentShieldMana += _shieldRegenRate * Time.deltaTime;

            _currentShieldMana = Mathf.Clamp(_currentShieldMana, 0, _maxShieldMana);

            UpdateShieldVisual();
        }
    }
    public void ShieldTakingDamage(int val)
    {
        _currentShieldMana -= val;
        _currentCoolDownTime = _coolDownPeriod;
        _currentShieldMana = Mathf.Max(_currentShieldMana, 0);
        if(_currentShieldMana <=0)
            _towerManager._onTowerTakingDamage?.Invoke();

        UpdateShieldVisual();
    }
    void UpdateShieldVisual()
    {
        if (_spriteRenderer == null) return;

        float normalized = _currentShieldMana / _maxShieldMana;
        normalized = Mathf.Pow(normalized, 1.5f); // tweak this
        shieldColour.a = normalized;
        _spriteRenderer.color = shieldColour;

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Ball>() != null)
        {
            Ball ball = other.GetComponent<Ball>();
            _deathVFX.SetActive(true);
            GlobalFeedbackManager.Instance.PlayGlobalFeedback?.Invoke();
            if (!ball._copyBall)
            {
                ball.OnBallReset?.Invoke();
            }
            else
            {
                ball.OnBallDestroy?.Invoke();
            }
        }
        if(other.GetComponent<PaddleHealth>() != null)
        {
            PaddleHealth ph = other.GetComponent<PaddleHealth>();
            ph.OnPaddleDisable?.Invoke();
        }
        if( other.GetComponent<TowerEssence>() != null)
        {
            TowerEssence te = other.GetComponent<TowerEssence>();
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_essenceDestroyed, transform.position);
            te.gameObject.SetActive(false);
        }
        if (other.GetComponent<BrickBar>() != null)
        {
            BrickBar _bb = other.GetComponent<BrickBar>();
            ShieldTakingDamage(_bb.GetShieldDamageValue());
            _bb.OnDamage(999,DeathCause.TOWER);
        }
    }
    public void AddShieldValue(int val) => _maxShieldMana += val;
    public void MinusShieldValue(int val) => _maxShieldMana -= val;
    public void MultipleMinusShieldValue(float val) => _maxShieldMana *= val;
    public void AddShieldRegenRate(int val) => _shieldRegenRate += val;
    public void MinusShieldRegenRate(int val) => _shieldRegenRate -= val;
    public void AddShieldCooldown(int val) => _coolDownPeriod += val;
    public void MinusShieldCooldown(int val) => _coolDownPeriod -= val;
    public void ResetShield() => _currentShieldMana = _maxShieldMana;
}
