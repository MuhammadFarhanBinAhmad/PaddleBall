using UnityEngine;

[CreateAssetMenu(fileName = "SO_BrickSpecialEffect", menuName = "Brick/SO_BrickSpecialEffect")]
public class SO_BrickSpecialEffect : ScriptableObject
{
    [Header("Lerp")]
    public AnimationCurve _effectLerp;
    public float _startScaleMultiplier;
    public float _endScaleMultiplier;
    public float _lerpDuration;

    public bool ReferenceObjectScale;
}
