using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targeting : MonoBehaviour
{
    List<Targetable> allTargetables = new();
    public float screenCenterThreshold = 0.1f;
    public static Targeting Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        }
        else
        {
            Destroy(this);
        }
    }

    public void RegisterTarget(Targetable target)
    {
        if (allTargetables == null) { allTargetables = new(); }
        if (!allTargetables.Contains(target))
        {
            allTargetables.Add(target);
        }
    }

    public void UnregisterTarget(Targetable target)
    {
        if (allTargetables.Contains(target))
        {
            allTargetables.Remove(target);
        }
    }

    public Targetable GetClosestTarget(List<Targetable> listOfAllObjects, Vector3 referencePosition, float maxDistance)
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

    public List<(Targetable target, Vector3 screenPoint)> FindTargetsOnScreen(Camera camera, Vector3 position, float maxDistance)
    {
        List<(Targetable target, Vector3 screenPoint)> targetsOnScreen = new();
        foreach (var target in allTargetables)
        {
            if (Vector3.SqrMagnitude(position - target.transform.position) > (maxDistance * maxDistance)) { continue; }
            Vector3 screenPoint = camera.WorldToViewportPoint(target.transform.position);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
            {
                targetsOnScreen.Add((target, screenPoint));
            }
        }
        return targetsOnScreen;
    }


    // This is likely to end up being expensive to run every frame with large numbers of targets (100+)
    public Targetable GetClosestTargetOnScreen(Camera camera, Vector3 position, float maxDistance, float screenCenterThreshold)
    {
        this.screenCenterThreshold = screenCenterThreshold;
        List<(Targetable target, Vector3 screenPoint)> targetsOnScreen = FindTargetsOnScreen(camera, position, maxDistance);
        Targetable closestTarget = null;
        float closestScreenDistance = screenCenterThreshold;
        float aspectRatio = Screen.width / (float)Screen.height;
        foreach (var (target, screenPoint) in targetsOnScreen)
        {
            float screenDistance = Vector2.Distance(new Vector2(screenPoint.x * aspectRatio, screenPoint.y), new Vector2(0.5f * aspectRatio, 0.5f));
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
        float screenHeight = canvas.referenceResolution.y * canvas.scaleFactor;
        if (TryGetComponent<Image>(out var circularImage))
        {
            circularImage.rectTransform.sizeDelta = new Vector2(screenHeight * adjustedScale, screenHeight * adjustedScale);
        }
    }
}