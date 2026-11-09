using System.Collections;
using UnityEngine;

public class TutorialOrder_EndDayIntro : TutorialOrder
{
    [SerializeField] GameObject _button;
    protected override void GiveInstruction()
    {
        _button.SetActive(false);
    }

    protected override IEnumerator WaitForAction()
    {
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
        TimeManager.ResetTimeScale();
    }


}
