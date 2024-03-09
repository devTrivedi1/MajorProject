using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestTargetable : Targetable, IResettable
{
   [SerializeField] int exampleValue = 15;
   [SerializeField] Targetable exampleTarget = null;
    public List<System.Action> UnityResetActions { get; set; } = new();
    public List<System.Action> MultithreadResetActions { get; set; } = new();

   private void Start()
   {
       var startingTarget = exampleTarget;
       var startingValue = exampleValue;
       MultithreadResetActions.Add(() => ResetValue(startingValue, ref exampleValue));
       MultithreadResetActions.Add(() => ResetValue(startingTarget, ref exampleTarget));
       IResettable[] resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToArray();
       foreach (var resettable in resettables)
       {
           resettable.ResetObject();
       }
   }

   public void ResetValue<T>(T initialValue, ref T reference)
   {
       reference = initialValue;
   }

   [VInspector.Button]
   public void ResetObject()
   {
       foreach (var action in UnityResetActions)
       {
           action.Invoke();
       }
   }

   [VInspector.Button]
   public void SetValues()
   {
       exampleTarget = FindObjectOfType<Targetable>();
       exampleValue = Random.Range(0, 100);
   }
}