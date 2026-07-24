using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _text;
    [SerializeField] private SO_FeedbackEffect _feedback;

    [Header("Movement")]
    [SerializeField] private float _moveUpDistance = 1.2f;

    private Vector3 _startPos;
    private Vector3 _startScale;
    private Color _startColor;

    Coroutine _animationRoutine;

    private void Awake()
    {
        _startScale = transform.localScale;
        _startColor = _text.color;
    }

    private void Start()
    {
        if (_animationRoutine != null)
            StopCoroutine(_animationRoutine);

        _animationRoutine = StartCoroutine(Animate());
    }
    public void SetValue(int val) => _text.text = val.ToString();
    IEnumerator Animate()
    {
        _startPos = transform.localPosition;

        float timer = 0f;

        while (timer < _feedback.animationDuration)
        {
            float t = timer / _feedback.animationDuration;

            //-------------------------
            // Scale
            //-------------------------

            float curve = _feedback._animCurve.Evaluate(t);

            Vector3 start =
                _startScale * _feedback._startscaleMultiplier;

            Vector3 end =
                _startScale * _feedback._endscaleMultiplier;

            transform.localScale =
                Vector3.LerpUnclamped(start, end, curve);

            //-------------------------
            // Move upward
            //-------------------------

            transform.localPosition =
                Vector3.Lerp(
                    _startPos,
                    _startPos + Vector3.up * _moveUpDistance,
                    t);

            //-------------------------
            // Fade
            //-------------------------

            Color c = _startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            _text.color = c;

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset for pooling
        transform.localScale = _startScale;
        transform.localPosition = _startPos;

        Color reset = _startColor;
        reset.a = 1f;
        _text.color = reset;

        Destroy(gameObject);

        _animationRoutine = null;
    }

    /// <summary>
    /// Call this when displaying the damage text.
    /// Maybe use later if i want to pool it?????
    /// </summary>
    public void Show(string value, Color color)
    {
        _text.text = value;

        _startColor = color;
        _startColor.a = 1f;

        _text.color = _startColor;

        gameObject.SetActive(true);
    }
}