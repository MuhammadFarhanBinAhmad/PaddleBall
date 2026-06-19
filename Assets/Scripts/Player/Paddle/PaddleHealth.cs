using System;
using System.Collections;
using UnityEngine;

public class PaddleHealth : MonoBehaviour
{
    Ball _ball;
    PaddleMovement _paddleMovement;
    PaddleVacoom _paddleVacoom;
    PaddleFeedbackManager _paddleFeedbackManager;

    SpriteRenderer _spriteRenderer;

    [Header("Respawn")]
    public Transform _spawnPos;
    public GameObject _deathVFX;
    public float _timeTillRespawn;
    bool _isPaddleDead;

    public Action OnPaddleDisable;
    public Action OnPaddleEnable;
    public Action<bool> SetBoolOnPaddleDisable;

    [Header("Knockback")]
    [SerializeField] float _knockbackDistance = 0.6f;
    [SerializeField] float _knockbackDuration = 0.15f;
    bool _isKnockbacking;

    public GameObject _hat;

    private void Awake()
    {
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();
        _ball = FindAnyObjectByType<Ball>();   
        _paddleFeedbackManager = FindAnyObjectByType<PaddleFeedbackManager>();

        _spriteRenderer = GetComponentInParent<SpriteRenderer>();

        OnPaddleDisable += DisablePaddle;
        OnPaddleDisable += StartRespawnPaddleTimer;
        OnPaddleDisable += PlayPaddleDisableAudio;

        OnPaddleEnable += EnablePaddle;
        OnPaddleEnable += PlayPaddleEnableAudio;

        SetBoolOnPaddleDisable += _paddleMovement.DisblePaddleMovement;
        SetBoolOnPaddleDisable += _paddleMovement.DisblePaddleCollider;
        SetBoolOnPaddleDisable += _paddleVacoom.DisableVacoom;


    }
    private void OnDisable()
    {
        OnPaddleDisable -= DisablePaddle;
        OnPaddleDisable -= StartRespawnPaddleTimer;
        OnPaddleDisable -= PlayPaddleDisableAudio;

        OnPaddleEnable -= EnablePaddle;
        OnPaddleEnable -= PlayPaddleEnableAudio;

        SetBoolOnPaddleDisable -= _paddleMovement.DisblePaddleMovement;
        SetBoolOnPaddleDisable -= _paddleMovement.DisblePaddleCollider;
        SetBoolOnPaddleDisable -= _paddleVacoom.DisableVacoom;

    }

    void DisablePaddle()
    {
        SetBoolOnPaddleDisable?.Invoke(true);
        _hat.SetActive(false);
        _spriteRenderer.enabled = false;
        _isPaddleDead = true;
        _deathVFX.SetActive(true);
        _paddleFeedbackManager.OnBeingDestroyed.Invoke();
    }

    void EnablePaddle()
    {
        SetBoolOnPaddleDisable?.Invoke(false);
        _hat.SetActive(true);
        _spriteRenderer.enabled = true;
        _isPaddleDead = false;
        transform.parent.position = _spawnPos.position;
        _paddleFeedbackManager.OnRespawn?.Invoke();
    }

    void StartRespawnPaddleTimer()
    {
        StartCoroutine(RespawnPaddle());
    }
    IEnumerator RespawnPaddle()
    {
        yield return new WaitForSeconds(_timeTillRespawn);
        OnPaddleEnable?.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (_isKnockbacking) return;

        if (other.CompareTag("Brick")&& _spriteRenderer.enabled)
        {
            _paddleFeedbackManager.OnBeingKnockBack?.Invoke();
            SetBoolOnPaddleDisable?.Invoke(true);
            other.GetComponentInChildren<BrickBar>().OnDeathByBrick();
            StartCoroutine(Knockback());
        }
        if (other.CompareTag("EnemyProjectile") && _spriteRenderer.enabled)
        {
            _paddleFeedbackManager.OnBeingKnockBack?.Invoke();
            SetBoolOnPaddleDisable?.Invoke(true);
            other.GetComponent<EnemyProjectile>().HandleProjectileDeath();
            _ball.OnBallReset?.Invoke();
            StartCoroutine(Knockback());
        }
    }
    public void PlayPaddleDisableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleDestroy, transform.position);
    public void PlayPaddleEnableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleRespawn, transform.position);
    public bool IsPaddleDead() => _isPaddleDead;
    IEnumerator Knockback()
    {
        _isKnockbacking = true;


        Vector3 startPos = transform.parent.position;
        Vector3 targetPos = startPos + Vector3.down * _knockbackDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _knockbackDuration;
            transform.parent.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // re-enable control
        SetBoolOnPaddleDisable?.Invoke(false);

        _isKnockbacking = false;
    }
}
