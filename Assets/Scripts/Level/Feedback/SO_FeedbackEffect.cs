using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "SO_FeedbackEffect", menuName = "Global Feedback/Feedbackeffect")]
public class SO_FeedbackEffect : ScriptableObject
{
    [Header("CamShake")]
    public float _duration;
    public float _trauma;
    [Header("LerpSize")]
    public AnimationCurve _animCurve;
    public float animationDuration;
    public float _startscaleMultiplier;
    public float _endscaleMultiplier;
    [Header("ParticleEffects")]
    public GameObject _vfxEffect;
    [Header("Audio")]
    [field: SerializeField] public EventReference sfx_feedback { get; private set; }
}
