using System;
using UnityEngine;

public static class Events
{
    public static event Action<GameObject> BuildSelected;
    public static event Action<Vector3Int, Vector3, bool> BuildTargetChanged;
    public static event Action<Vector3Int> Constructed;
    public static event Action BuildCanceled;
    public static event Action Fliped;
    public static event Action HalfFliped;

    public static void CallBuildSelected(GameObject buildingPrefab)
    {
        BuildSelected?.Invoke(buildingPrefab);
    }

    public static void CallBuildTargetChanged(Vector3Int cellPosition, Vector3 worldPosition, bool canBuild)
    {
        BuildTargetChanged?.Invoke(cellPosition, worldPosition, canBuild);
    }

    public static void CallConstructed(Vector3Int cellPosition)
    {
        Constructed?.Invoke(cellPosition);
    }

    public static void CallBuildCanceled()
    {
        BuildCanceled?.Invoke();
    }

    public static void CallFliped()
    {
        Fliped?.Invoke();
    }

    public static void CallHalfFliped()
    {
        HalfFliped?.Invoke();
    }
}
