using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum CUTSCENETYPE
{
    NONE = 0,
    TEXT = 1 << 0,
    ANIMATION = 1 << 1,
    POPIN = 1 << 2,
    PARTICLE_EFFECT = 1 << 3,
    CAMSHAKE = 1 << 4,
    DESTROY_BOSS = 1 << 5,
}

[CreateAssetMenu(fileName = "SO_TalkingBubbleContent", menuName = "Cutscene/SO_TalkingBubbleContent")]
public class SO_CutSceneEventContent : ScriptableObject
{

    public CUTSCENETYPE type;

    [Header("TEXT")]
    public string _speakerName;
    [TextArea(3, 10)]
    public List<string> _dialougeTexts = new();

    [Header("POPIN")]
    public AnimationCurve _popInEffectLerp;
    public float _startScaleMultiplier;
    public float _endScaleMultiplier;
    public float _animationPopInDuration;

    [Header("PARTICLE_EFFECT")]
    public GameObject _particleEffectPrefab;

    [Header("CAM_SHAKE")]
    public float _duration;
    public float _trauma;

    [Header("MISC")]
    public float _timeDelay;

}
