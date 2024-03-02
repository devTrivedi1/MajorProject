using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targeting : MonoBehaviour
{
    static List<Targetable> allTargetables = new();

    public float screenCenterThreshold = 0.1f;
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
        List<Targetable> targetsOnScreen = new();
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

    public static Targetable GetClosestTargetOnScreen(Camera camera, Vector3 position, float maxDistance, float screenCenterThreshold)
    {
        List<Targetable> targetsOnScreen = FindTargetsOnScreen(camera);
        Targetable closestTarget = null;
        float closestScreenDistance = screenCenterThreshold;
        foreach (var target in targetsOnScreen)
        {
            if (Vector3.Distance(position, target.transform.position) > maxDistance) { continue; }
            Vector3 screenPoint = camera.WorldToViewportPoint(target.transform.position);
            float screenDistance = Vector2.Distance(screenPoint, new Vector2(0.5f, 0.5f));
            if (closestScreenDistance > screenDistance)
            {
                closestTarget = target;
                closestScreenDistance = screenDistance;
            }
        }
        return closestTarget;
    }


    void OnDrawGizmos()
    {
        float adjustedScale = screenCenterThreshold / 0.5f;
        CanvasScaler canvas = GetComponentInParent<CanvasScaler>();
        float screenWidth = canvas.referenceResolution.x * canvas.scaleFactor;
        float screenHeight = canvas.referenceResolution.y * canvas.scaleFactor;
        if (TryGetComponent<Image>(out var circularImage))
        {
            circularImage.rectTransform.sizeDelta = new Vector2(screenWidth * adjustedScale, screenHeight * adjustedScale);
        }
    }
}