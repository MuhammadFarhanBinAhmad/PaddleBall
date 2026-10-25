
using UnityEngine;

public class Apprentice_MeteorWarning : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Fade Settings")]
    [SerializeField, Range(0f, 1f)] private float _minAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float _maxAlpha = 1f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _timeBeforeDestroy = 5f;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve _fadeCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float _timer;

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, _timeBeforeDestroy);
    }

    private void Update()
    {
        if (_spriteRenderer == null || _fadeDuration <= 0f)
            return;

        _timer += Time.deltaTime;

        // Continuously loops:
        // 0 -> 1 -> 0 -> 1 -> ...
        float t = Mathf.PingPong(
            _timer / _fadeDuration,
            1f
        );

        float curveValue = _fadeCurve.Evaluate(t);

        float alpha = Mathf.Lerp(
            _minAlpha,
            _maxAlpha,
            curveValue
        );

        // Only modify alpha, preserving RGB.
        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }
}
