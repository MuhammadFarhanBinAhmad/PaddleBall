using System;
using UnityEngine;

public class GlobalGameplayEventManager : MonoBehaviour
{
    public static Action<ExplosionContext> OnClusterBombExplode;
}
