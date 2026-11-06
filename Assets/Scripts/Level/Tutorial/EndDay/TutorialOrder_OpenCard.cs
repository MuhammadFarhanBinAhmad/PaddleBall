using System.Collections;
using UnityEngine;

public class TutorialOrder_OpenCard : TutorialOrder
{
    [Header("Spell Book")]
    [SerializeField] StoreOverlayUI _storeOverlayUI;

    bool _pageClicked;

    protected override void GiveInstruction()
    {
    }

    protected override IEnumerator WaitForAction()
    {
        if (_storeOverlayUI == null)
        {
            Debug.LogError(
                "TutorialOrder_OpenSpellBook: StoreOverlayUI reference is missing."
            );

            yield break;
        }

        _pageClicked = false;

        // Subscribe to the button click event.
        _storeOverlayUI.OnOpenCardClicked += HandlePageClicked;
        // Wait until the player clicks the button.
        while (!_pageClicked)
        {
            yield return null;
        }

        // Stop listening once the action is complete.
        _storeOverlayUI.OnOpenCardClicked -= HandlePageClicked;
        _worldToCanvasPosition.ToggleFocus(false);
        yield return new WaitForSecondsRealtime(_delayBeforeNextStep);
    }

    private void HandlePageClicked()
    {
        _pageClicked = true;
        Debug.Log("Spell Book button clicked!");
    }

    private void OnDisable()
    {
        // Clean up if the tutorial order is interrupted.
        if (_storeOverlayUI != null)
        {
            _storeOverlayUI.OnOpenCardClicked -= HandlePageClicked;
        }
    }
}
