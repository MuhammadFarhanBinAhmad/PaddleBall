using System.Collections;
using UnityEngine;

public class TutorialOrder_HitPaddle : TutorialOrder
{
    [SerializeField] PaddleReflection _paddleReflection;

    protected override void GiveInstruction()
    {
        print("The ball can bounce off Paddle. Try it");
    }

    protected override IEnumerator WaitForAction()
    {
        if (_paddleReflection == null)
        {
            Debug.LogError(
                "PaddleReflection is not assigned!"
            );

            yield break;
        }

        bool ballHitPaddle = false;

        void HandleBallHit(Ball ball)
        {
            ballHitPaddle = true;
        }

        _paddleReflection.OnBallHitPaddle += HandleBallHit;

        while (!ballHitPaddle)
        {
            yield return null;
        }

        _paddleReflection.OnBallHitPaddle -= HandleBallHit;
    }
}
