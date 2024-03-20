using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class Resetter : MonoBehaviour
{
    private struct ObjectState
    {
        public Dictionary<FieldInfo, object> FieldStates;
        public bool Enabled;
        public List<ComponentState> ComponentStates;
    }

    private Dictionary<IResettable, ObjectState> allInitialStates = new();

    void Start()
    {
        float startTime = Time.realtimeSinceStartup;
        CaptureInitialStates();
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("Time to capture: " + (endTime - startTime) + " seconds. That is " + Mathf.Round(1 / (endTime - startTime)) + "FPS.");
    }

    public void CaptureInitialStates()
    {
        var resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>();
        foreach (var resettable in resettables)
        {
            var componentStates = BoilerplateHell(resettable);

            var initialState = new ObjectState
            {
                FieldStates = new Dictionary<FieldInfo, object>(),
                Enabled = resettable.enabled,
                ComponentStates = componentStates
            };

            var type = resettable.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<ResettableAttribute>() != null)
                {
                    initialState.FieldStates[field] = field.GetValue(resettable);
                }
            }

            allInitialStates[resettable] = initialState;
        }
    }

    public void ResetAll()
    {
        float startTime = Time.realtimeSinceStartup;
        foreach (var resettable in allInitialStates.Keys)
        {
            var initialState = allInitialStates[resettable];
            foreach (var entry in initialState.FieldStates)
            {
                entry.Key.SetValue(resettable, entry.Value);
            }

            MonoBehaviour mono = (resettable as MonoBehaviour);
            mono.StopAllCoroutines();
            (resettable as MonoBehaviour)?.StopAllCoroutines();
            resettable.enabled = initialState.Enabled;

            foreach (var componentState in initialState.ComponentStates)
            {
                componentState.ResetState();
            }
        }
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("Time to reset: " + (endTime - startTime) + " seconds. That is " + Mathf.Round(1 / (endTime - startTime)) + "FPS.");
    }

    public List<ComponentState> BoilerplateHell(IResettable resettable)
    {
        var componentState = new List<ComponentState>();

        if (resettable is IResettableGO)
        {
            var go = new GameObjectState();
            go.CaptureState(resettable);
            componentState.Add(go);
        }

        if (resettable is IResettableRb)
        {
            var rb = new RigidbodyState();
            rb.CaptureState(resettable);
            componentState.Add(rb);
        }

        if (resettable is IResettableTransform)
        {
            var transform = new TransformState();
            transform.CaptureState(resettable);
            componentState.Add(transform);
        }

        //More Unity components to be added as needed
        return componentState;
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ResettableAttribute : Attribute
{
}