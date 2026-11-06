using System.Collections;
using UnityEngine;

public class TutorialOrder_EndDayIntro : TutorialOrder
{
    [SerializeField] GameObject _button;
    protected override void GiveInstruction()
    {
        
    }

    protected override IEnumerator WaitForAction()
    {
        _button.SetActive(false);
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
        TimeManager.ResetTimeScale();
    }


}
