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
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public bool Active;
        // Add more Unity component states here as needed
    }

    private Dictionary<IResettable, ObjectState> allInitialStates = new();

    void Start()
    {
        CaptureInitialStates();
    }

    public void CaptureInitialStates()
    {
        var resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>();
        foreach (var resettable in resettables)
        {
            var initialState = new ObjectState
            {
                FieldStates = new Dictionary<FieldInfo, object>(),
                Position = resettable.transform.position,
                Rotation = resettable.transform.rotation,
                Scale = resettable.transform.localScale,
                Active = resettable.gameObject.activeSelf
                // Capture other Unity component states as needed
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
        foreach (var resettable in allInitialStates.Keys)
        {
            var initialState = allInitialStates[resettable];
            foreach (var entry in initialState.FieldStates)
            {
                entry.Key.SetValue(resettable, entry.Value);
            }

            resettable.transform.position = initialState.Position;
            resettable.transform.rotation = initialState.Rotation;
            resettable.transform.localScale = initialState.Scale;
            resettable.gameObject.SetActive(initialState.Active);
            // Reset other Unity component states as needed
        }
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ResettableAttribute : Attribute
{
}