using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DeadZone : MonoBehaviour
{
    TowerManager _towerManager;

    public Action OnShieldDamage;

    public GameObject _deathVFX;

    [Header("Shield")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _maxTowerAndPaddleHealth;
    [SerializeField] float _currentTowerAndPaddleHealth;
    [Header("Feedback")]
    [SerializeField] SO_FeedbackEffect so_OnBallHit;
    [SerializeField] SO_FeedbackEffect so_OnShieldHit;
    [SerializeField] SO_FeedbackEffect so_OnShieldDown;

    Color shieldColour;

    private void Awake()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
        shieldColour = _spriteRenderer.color;
        _currentTowerAndPaddleHealth = _maxTowerAndPaddleHealth;
    }
    private void Start()
    {
        OnShieldDamage += UpdateShieldVisual;


    }

    private void OnDestroy()
    {
        OnShieldDamage -= UpdateShieldVisual;
    }
    
    public void ShieldTakingDamage(int val)
    {
        int dma = Math.Max(1, val);
        _currentTowerAndPaddleHealth -= dma;
        _currentTowerAndPaddleHealth = Mathf.Max(_currentTowerAndPaddleHealth, 0);

        GlobalFeedbackManager.Instance.SetFeedbackValue(so_OnShieldHit);


        GlobalFeedbackManager.Instance.PlayGlobalFeedback();
        OnShieldDamage?.Invoke();
    }
    void UpdateShieldVisual()
    {
        if (_spriteRenderer == null) return;

        float normalized = _currentTowerAndPaddleHealth / _maxTowerAndPaddleHealth;
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
            GlobalFeedbackManager.Instance.SetFeedbackValue(so_OnBallHit);
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
        //if(other.GetComponent<PaddleHealth>() != null)
        //{
        //    PaddleHealth ph = other.GetComponent<PaddleHealth>();
        //    ph.OnPaddleDisable?.Invoke();
        //}
        if( other.GetComponent<TowerEssence>() != null)
        {
            TowerEssence te = other.GetComponent<TowerEssence>();
            AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_essenceDestroyed, transform.position);
            te.gameObject.SetActive(false);
        }
        if (other.GetComponent<BrickHealthComponent>() != null)
        {
            BrickHealthComponent _bb = other.GetComponent<BrickHealthComponent>();
            ShieldTakingDamage(_bb._brickBar.GetLayer());
            _bb.OnDamage(999,STATUSTYPE.NONE,DeathCause.TOWER,true);
        }
        //if(other.CompareTag("EnemyProjectile"))
        //{
        //    EnemyProjectile ep = other.GetComponent<EnemyProjectile>();
        //    ep.HandleProjectileDeath();
        //    ShieldTakingDamage(ep.GetDamage(),1);//Need set value for enemy projectile

        //}
    }
    public float GetCurrentShield() => _currentTowerAndPaddleHealth;
    public float GetMaxShield() => _maxTowerAndPaddleHealth;
    public float GetShieldPercentage() => _currentTowerAndPaddleHealth / _maxTowerAndPaddleHealth;
    public void AddShieldValue(int val) => _maxTowerAndPaddleHealth += val;
    public void MinusShieldValue(int val) => _maxTowerAndPaddleHealth -= val;
    public void MultipleMinusShieldValue(float val) => _maxTowerAndPaddleHealth *= val;
    public void ResetShield() => _currentTowerAndPaddleHealth = _maxTowerAndPaddleHealth;
}
