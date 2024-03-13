using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestTargetable : Targetable, IResettable
{
   [SerializeField] int exampleValue = 15;
   [SerializeField] Targetable exampleTarget = null;
    public List<System.Action> ResetActions { get; set; } = new();

   private void Start()
   {
        var startingTarget = exampleTarget;
        var startingValue = exampleValue;
        var startingPosition = transform.position;
        ResetActions.Add(() => exampleValue = startingValue);
        ResetActions.Add(() => exampleTarget = startingTarget);
        ResetActions.Add(() => transform.position = startingPosition);
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