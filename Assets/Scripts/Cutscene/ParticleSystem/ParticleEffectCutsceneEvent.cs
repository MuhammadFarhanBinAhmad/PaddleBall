using System.Collections;
using UnityEngine;

public class ParticleEffectCutsceneEvent : BaseCutsceneEvent
{
    private GameObject _particleSystem;
    private Coroutine _routine;

    public override void SetUpContent(SO_CutSceneEventContent content)
    {
        base.SetUpContent(content);
    }

    public override void ExecuteEvent()
    {
        if (_particleSystem == null)
        {
            EndEvent();
            return;
        }

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(PlayParticleEffect());
    }

    private IEnumerator PlayParticleEffect()
    {
        _particleSystem.SetActive(true);

        ParticleSystem[] systems = _particleSystem.GetComponentsInChildren<ParticleSystem>(true);

        if (systems.Length == 0)
        {
            Debug.LogWarning($"{_particleSystem.name} contains no ParticleSystem components.");
            EndEvent();
            yield break;
        }

        // Restart all systems
        foreach (ParticleSystem ps in systems)
        {
            ps.Clear();
            ps.Play();
        }

        // Wait one frame so IsAlive() updates correctly
        yield return null;

        // Wait until ALL particle systems are dead
        bool alive = true;

        while (alive)
        {
            alive = false;

            foreach (ParticleSystem ps in systems)
            {
                if (ps != null && ps.IsAlive(true))
                {
                    alive = true;
                    break;
                }
            }

            yield return null;
        }

        EndEvent();
    }

    public override void EndEvent()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        _particleSystem = null;
        NotifyFinished();
    }

    public void SetParticleSystem(GameObject system)
    {
        _particleSystem = system;
    }
}
