using UnityEngine;

public abstract class BaseBossFeedbackManager : MonoBehaviour
{
    public SOLerpAnimationEffect _damageLerpAnim;
    AnimationCurveEffect _damageLerpEffect;

    private void Awake()
    {
        _damageLerpEffect = GetComponent<AnimationCurveEffect>();
    }

    public virtual void OnBallHit()
    {
        print("hit");
        _damageLerpEffect.PlayEffect(_damageLerpAnim, this.gameObject);
    }
}
