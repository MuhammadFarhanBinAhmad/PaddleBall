
using System.Collections;
using UnityEngine;

public class TutorialOrder_OpenSpellBook : TutorialOrder
{
    [Header("Spell Book")]
    [SerializeField] StoreOverlayUI _storeOverlayUI;

    bool _pageClicked;

    protected override void GiveInstruction()
    {
        Debug.Log("Open the Spell Book!");
    }

    protected override IEnumerator WaitForAction()
    {
        _worldToCanvasPosition.ToggleFocus(false);
        if (_storeOverlayUI == null)
        {
            Debug.LogError(
                "TutorialOrder_OpenSpellBook: StoreOverlayUI reference is missing."
            );

            yield break;
        }

        _pageClicked = false;

        // Subscribe to the button click event.
        _storeOverlayUI.OnSelectAbilityPageClicked += HandlePageClicked;

        // Wait until the player clicks the button.
        while (!_pageClicked)
        {
            yield return null;
        }

        // Stop listening once the action is complete.
        _storeOverlayUI.OnSelectAbilityPageClicked -= HandlePageClicked;

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
            _storeOverlayUI.OnSelectAbilityPageClicked -= HandlePageClicked;
        }
    }
}
