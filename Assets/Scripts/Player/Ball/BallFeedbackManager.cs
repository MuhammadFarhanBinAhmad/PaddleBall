using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BallFeedbackManager : MonoBehaviour
{
    Ball _ballManager;

    [Header("Hit")]
    [SerializeField] GameObject _hitEffectVFX;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _startSpirite,_hitSprite;
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

        _changeSpriteCoroutine = StartCoroutine(AnimateSpriteChange());

    }
    IEnumerator AnimateSpriteChange()
    {
        _spriteRenderer.sprite = _hitSprite;
        yield return new WaitForSeconds(_spriteChangeTime);
        _spriteRenderer.sprite = _startSpirite;

        Quaternion originalRotation = transform.rotation;

        float randomZ = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomZ);
    }

    public void PlayHitWallAudio() => AudioManager.Instance.PlayOneShot(FmodEvent.Instance.sfx_onBallHitWall, transform.position);
}
