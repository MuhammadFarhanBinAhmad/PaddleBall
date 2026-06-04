using UnityEngine;

public class HomingTarget : ABSAbility
{
    [SerializeField]float _homingStrength;

    private void Start()
    {
        Ball ball = FindAnyObjectByType<Ball>();
        ball.IncreaseHomingStrength(_homingStrength);

    }
}
