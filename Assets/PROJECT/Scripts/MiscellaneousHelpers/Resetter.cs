using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;

public class Resetter : MonoBehaviour
{
    Dictionary<Type, Func<ComponentState>> interfaceCorrelation = new() 
    {
        { typeof(IResettableGO), () => new GameObjectState() },
        { typeof(IResettableRb), () => new RigidbodyState() },
        { typeof(IResettableTransform), () => new TransformState() }
        //More Unity component states to be added as needed
    };

    private struct ObjectState
    {
        public Dictionary<FieldInfo, object> FieldStates;
        public bool Enabled;
        public List<ComponentState> ComponentStates;
    }

    private Dictionary<IResettable, ObjectState> allInitialStates = new();

    void Start()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        float startTime = Time.realtimeSinceStartup;
        CaptureInitialStates();
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("Time to capture: " + (endTime - startTime) + " seconds. That is " + Mathf.Round(1 / (endTime - startTime)) + "FPS.");
    }

    void CaptureInitialStates()
    {
        var resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>();
        foreach (var resettable in resettables)
        {
            var componentStates = GetComponentStates(resettable);

            var initialState = new ObjectState
            {
                FieldStates = new Dictionary<FieldInfo, object>(),
                Enabled = resettable.enabled,
                ComponentStates = componentStates
            };

            var type = resettable.GetType();
            var fields = GetAllFields(type);

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

            if (resettable is MonoBehaviour monoBehaviour) { monoBehaviour.StopAllCoroutines(); }
            resettable.enabled = initialState.Enabled;

            foreach (var componentState in initialState.ComponentStates)
            {
                componentState.ResetState();
            }
        }
        float endTime = Time.realtimeSinceStartup;
        Debug.Log("Time to reset: " + (endTime - startTime) + " seconds. That is " + Mathf.Round(1 / (endTime - startTime)) + "FPS.");
    }

    // Recursive function that returns ALL fields up to the base class, as long as it is an IResettable
    IEnumerable<FieldInfo> GetAllFields(Type t)
    {
        if (t == null || !typeof(IResettable).IsAssignableFrom(t))
        {
            return Enumerable.Empty<FieldInfo>();
        }

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
    }

    List<ComponentState> GetComponentStates(IResettable resettable)
    {
        var componentStates = new List<ComponentState>();

        foreach (var iResettable in resettable.GetType().GetInterfaces())
        {
            if (interfaceCorrelation.TryGetValue(iResettable, out Func<ComponentState> newComponentState))
            {
                ComponentState componentState = newComponentState();
                componentState.CaptureState(resettable);
                componentStates.Add(componentState);
            }
        }

        return componentStates;
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ResettableAttribute : Attribute
{
}