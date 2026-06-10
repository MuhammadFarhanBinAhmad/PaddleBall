using UnityEngine;

public abstract class BaseBossBrick : MonoBehaviour
{
    internal BrickHealthComponent _brickHealthComponent;

    private void Awake()
    {
        _brickHealthComponent = GetComponent<BrickHealthComponent>();
    }

    internal abstract void HandleDamage(int damage);
    internal abstract void DamageFeedback();
}
