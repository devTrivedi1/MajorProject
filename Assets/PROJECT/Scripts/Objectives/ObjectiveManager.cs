using System;
using CustomInspector;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour, IResettable
{
    public Action ObjectiveComplete;
    [Resettable] bool isObjectiveComplete;
    [SerializeField] ObjectiveType type;
    [SerializeField, ReadOnly] Objective objective;

    void Update()
    {
        if (!isObjectiveComplete && objective.CheckCompletion())
        {
            isObjectiveComplete = true;
            ObjectiveComplete?.Invoke();
            Debug.Log($"Objective '{type}' complete!");
        }
    }

    public enum ObjectiveType
    {
        None,
        DestroyEnemies,
        DestroyTargets,
        CollectItems
    }

    private void OnDrawGizmos()
    {
        Type objectiveType = Type.GetType(type.ToString());
        if (objectiveType != null)
        {
            if (objective != null && objective.GetType() != objectiveType)
            {
                DestroyImmediate(objective, true);
                objective = null;
            }

            if (objective == null)
            {
                objective = (Objective)gameObject.AddComponent(objectiveType);
            }
        }
        else if (objective != null) { DestroyImmediate(objective, true); }
    }
}