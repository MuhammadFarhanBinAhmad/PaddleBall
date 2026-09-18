using System;
using UnityEngine;

public class ClusterBombAbility : ABSAbility
{
    ClusterBombPool _clusterBombPool;

    private void Start()
    {
        FindAnyObjectByType<SharpnelAbility>().ConvertSharpnel();

    }
}
