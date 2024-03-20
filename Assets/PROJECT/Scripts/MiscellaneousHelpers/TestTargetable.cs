using UnityEngine;

public class TestTargetable : Targetable, IResettable, IResettableGO, IResettableTransform
{
    [Resettable][SerializeField] private int exampleValue = 15;
    [Resettable][SerializeField] private Targetable exampleTarget = null;

    [VInspector.Button]
    public void ResetObject()
    {
        FindObjectOfType<Resetter>().ResetAll();
    }

   [VInspector.Button]
   public void SetValues()
   {
        exampleTarget = FindObjectOfType<Targetable>();
        exampleValue = Random.Range(0, 100);
   }
}