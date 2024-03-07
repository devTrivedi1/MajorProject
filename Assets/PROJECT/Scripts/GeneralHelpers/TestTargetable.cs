using System.Collections.Generic;
using UnityEngine;

public class TestTargetable : Targetable, IResettable
{
   [SerializeField] int exampleValue = 15;
   [SerializeField] Targetable exampleTarget = null;
   public List<System.Action> ResetActions { get; set; } = new();

   private void Start()
   {
       var startingValue = exampleValue;
       var startingTarget = exampleTarget;
       ResetActions.Add(() => ResetValue(startingValue, ref exampleValue));
       ResetActions.Add(() => ResetValue(startingTarget, ref exampleTarget));
   }

   public void ResetValue<T>(T initialValue, ref T reference)
   {
       reference = initialValue;
   }

   [VInspector.Button]
   public void ResetObject()
   {
       foreach (var action in ResetActions)
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