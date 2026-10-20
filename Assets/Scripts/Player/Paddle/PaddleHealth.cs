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

    private void Awake()
    {
        _paddleMovement = FindAnyObjectByType<PaddleMovement>();
        _paddleVacoom = FindAnyObjectByType<PaddleVacoom>();
        _ball = FindAnyObjectByType<Ball>();   
        _paddleFeedbackManager = FindAnyObjectByType<PaddleFeedbackManager>();
        _deadZone = FindAnyObjectByType<DeadZone>();

        _spriteRenderer = GetComponentInParent<SpriteRenderer>();



    }
    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Brick"))
        {
            BrickHealthComponent _bhc = other.GetComponentInChildren<BrickHealthComponent>();
            _bhc.OnDeathByBrick();
            _deadZone.ShieldTakingDamage(_bhc._brickBar.GetLayer());
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
