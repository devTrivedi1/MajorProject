using System;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public Action ObjectiveComplete { get; private set; }
    bool isObjectiveComplete;

    protected abstract bool CheckCompletion();

    void Update()
    {
        if (!isObjectiveComplete && CheckCompletion())
        {
            isObjectiveComplete = true;
            ObjectiveComplete?.Invoke();
        }
    }
}