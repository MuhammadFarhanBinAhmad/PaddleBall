using System;
using System.Collections;
using UnityEngine;

public class PaddleHealth : MonoBehaviour
{
    DeadZone _deadZone;


    [Header("Respawn")]
    public Transform _spawnPos;
    public GameObject _deathVFX;
    public float _timeTillRespawn;
    bool _isPaddleDead;

    private void Awake()
    {
        _deadZone = FindAnyObjectByType<DeadZone>();

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
            BaseEnemyProjectile ep = other.GetComponent<BaseEnemyProjectile>();

            _deadZone.ShieldTakingDamage(ep.GetDamage());
            ep.HandleProjectileDeath();
        }

    }
    public void PlayPaddleDisableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleDestroy, transform.position);
    public void PlayPaddleEnableAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onPaddleRespawn, transform.position);
    public bool IsPaddleDead() => _isPaddleDead;

}
