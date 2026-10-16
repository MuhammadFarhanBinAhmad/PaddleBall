using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOrder_CollectEssence : TutorialOrder
{
    [Header("Collection Tracking")]
    [SerializeField] float _completionDelay = 0.5f;

    // Essence spawned during or already active when this order begins.
    readonly List<TowerEssence> _remainingEssence =
        new List<TowerEssence>();

    bool _hasTrackedEssence;

    protected override void GiveInstruction()
    {
        Debug.Log("Collect the Essence!");
    }

    protected override IEnumerator WaitForAction()
    {
        _remainingEssence.Clear();
        _hasTrackedEssence = false;

        // Listen for new Essence before taking the active-object snapshot.
        TowerEssence.OnEssenceSpawned += RegisterEssence;

        foreach (TowerEssence essence in TowerEssence.ActiveEssences)
        {
            RegisterEssence(essence);
        }

        // Wait for at least one Essence to be tracked, then for all
        // tracked Essence to be collected.
        while (!_hasTrackedEssence || _remainingEssence.Count > 0)
        {
            yield return null;
        }

        StopTracking();

        Debug.Log("All tutorial Essence collected!");

        // Give the player a moment before the next instruction.
        yield return new WaitForSecondsRealtime(_completionDelay);
    }

    private void RegisterEssence(TowerEssence essence)
    {
        if (essence == null)
            return;

        if (_remainingEssence.Contains(essence))
            return;

        _remainingEssence.Add(essence);
        _hasTrackedEssence = true;

        essence.OnCollected += HandleEssenceCollected;

        Debug.Log(
            $"Essence registered. Remaining: {_remainingEssence.Count}"
        );
    }

    private void HandleEssenceCollected(TowerEssence essence)
    {
        if (essence == null)
            return;

        essence.OnCollected -= HandleEssenceCollected;

        if (_remainingEssence.Remove(essence))
        {
            Debug.Log( 
                $"Essence collected. Remaining: {_remainingEssence.Count}"
            );
        }
    }

    private void StopTracking()
    {
        TowerEssence.OnEssenceSpawned -= RegisterEssence;

        foreach (TowerEssence essence in _remainingEssence)
        {
            if (essence != null)
                essence.OnCollected -= HandleEssenceCollected;
        }

        _remainingEssence.Clear();
    }

    private void OnDisable()
    {
        StopTracking();
    }
}