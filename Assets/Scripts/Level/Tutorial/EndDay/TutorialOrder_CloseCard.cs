using System.Collections;
using UnityEngine;

public class TutorialOrder_CloseCard : TutorialOrder
{
    [Header("Card Page")]
    [SerializeField] StoreOverlayUI _storeOverlayUI;

    bool _pageClicked;

    protected override void GiveInstruction()
    {
        Debug.Log("Close the Spell Book!");
    }

    protected override IEnumerator WaitForAction()
    {
        if (_storeOverlayUI == null)
        {
            Debug.LogError(
                "TutorialOrder_CloseSpellBook: StoreOverlayUI reference is missing."
            );

            yield break;
        }

        _pageClicked = false;

        // Subscribe to the close button click event.
        _storeOverlayUI.OnCloseCardClicked += HandlePageClicked;

        // Wait until the player clicks the close button.
        while (!_pageClicked)
        {
            yield return null;
        }

        // Stop listening once the action is complete.
        _storeOverlayUI.OnCloseCardClicked -= HandlePageClicked;

        TimeManager.StopTime();
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);

    }

    private void HandlePageClicked()
    {
        _pageClicked = true;
        Debug.Log("Spell Book close button clicked!");
    }

    private void OnDisable()
    {
        // Clean up if the tutorial order is interrupted.
        if (_storeOverlayUI != null)
        {
            _storeOverlayUI.OnCloseCardClicked -= HandlePageClicked;
        }
    }
}
