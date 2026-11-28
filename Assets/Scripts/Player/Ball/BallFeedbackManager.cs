using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BallFeedbackManager : MonoBehaviour
{
    Ball _ballManager;

    [Header("Hit")]
    [SerializeField] GameObject _hitEffectVFX;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _spriteChangeTime;
    Coroutine _changeSpriteCoroutine;
    private void Start()
    {
        _ballManager = FindAnyObjectByType<Ball>();

        _ballManager.OnBallHit += ChangeSpriteOnHit;
        _ballManager.OnBallHit += PlayHitWallAudio;


    }
    private void OnDestroy()
    {
        _ballManager.OnBallHit -= ChangeSpriteOnHit;
        _ballManager.OnBallHit -= PlayHitWallAudio;

    }
    public void ChangeSpriteOnHit()
    {
        if(_changeSpriteCoroutine != null) 
            StopCoroutine(_changeSpriteCoroutine);
        //_hitEffectVFX.SetActive(true);
    }

    public void PlayHitWallAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallHitWall, transform.position);
}
