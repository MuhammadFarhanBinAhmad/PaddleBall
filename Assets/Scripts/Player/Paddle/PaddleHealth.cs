using System;
using System.Collections;
using UnityEngine;

public class PaddleHealth : MonoBehaviour
{
    Ball _ball;
    PaddleMovement _paddleMovement;
    PaddleVacoom _paddleVacoom;
    PaddleFeedbackManager _paddleFeedbackManager;
    DeadZone _deadZone;

    SpriteRenderer _spriteRenderer;

    [Header("Respawn")]
    public Transform _spawnPos;
    public GameObject _deathVFX;
    public float _timeTillRespawn;
    bool _isPaddleDead;

    public Action OnPaddleDisable;
    public Action OnPaddleEnable;
    public Action<bool> SetBoolOnPaddleDisable;

    public GameObject _hat;

    private void Awake()
    {
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();
        _ball = FindAnyObjectByType<Ball>();   
        _paddleFeedbackManager = FindAnyObjectByType<PaddleFeedbackManager>();
        _deadZone = FindAnyObjectByType<DeadZone>();

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

        if (other.CompareTag("Brick"))
        {
            BrickHealthComponent _bhc = other.GetComponentInChildren<BrickHealthComponent>();
            BrickBar _bb = other.GetComponent<BrickBar>();
            _bhc.OnDeathByBrick();
            _deadZone.ShieldTakingDamage(_bb.GetLayer());
        }
        if (other.CompareTag("EnemyProjectile"))
        {
            EnemyProjectile ep = other.GetComponent<EnemyProjectile>();

            _deadZone.ShieldTakingDamage(ep.GetDamage());
            ep.HandleProjectileDeath();
        }
    }
    public void PlayPaddleDisableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleDestroy, transform.position);
    public void PlayPaddleEnableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleRespawn, transform.position);
    public bool IsPaddleDead() => _isPaddleDead;

}
