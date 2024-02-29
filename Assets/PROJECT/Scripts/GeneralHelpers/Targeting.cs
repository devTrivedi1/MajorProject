using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    static List<Targetable> allTargetables = new();

    public static void RegisterTarget(Targetable target)
    {
        if (allTargetables == null) { allTargetables = new(); }
        if (!allTargetables.Contains(target))
        {
            allTargetables.Add(target);
        }
    }

    public static void UnregisterTarget(Targetable target)
    {
        if (allTargetables.Contains(target))
        {
            allTargetables.Remove(target);
        }
    }

    public static Targetable GetClosestTarget(List<Targetable> listOfAllObjects, Vector3 referencePosition, float maxDistance)
    {
        Targetable nearestObject = default;
        float closestDistance = maxDistance;
        foreach (Targetable obj in listOfAllObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, referencePosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestObject = obj;
            }
        }
        return nearestObject;
    }

    public static List<Targetable> FindTargetsOnScreen(Camera camera)
    {
        List<Targetable> targetsOnScreen = new List<Targetable>();
        foreach (var target in allTargetables)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(target.transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                targetsOnScreen.Add(target);
            }
        }
        return targetsOnScreen;
    }

    public static Targetable GetClosestTargetOnScreen(Camera camera, Vector3 position, float maxDistance)
    {
        List<Targetable> targetsOnScreen = FindTargetsOnScreen(camera);
        Targetable closestTarget = null;
        float closestDistance = maxDistance;
        foreach (var target in targetsOnScreen)
        {
            float distance = Vector3.Distance(position, target.transform.position);
            if (distance < closestDistance)
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }
        return closestTarget;
    }
}