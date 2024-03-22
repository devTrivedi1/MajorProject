using UnityEngine;

public class EnvironmentalAidManager : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;
    private GrindController gc;

    void Start()
    {
        gc = FindObjectOfType<GrindController>();

        if (gc != null)
        {
            player = gc.gameObject;
            rb = player.GetComponent<Rigidbody>();
            EnviromentalAid[] aids = FindObjectsOfType<EnviromentalAid>();

            foreach (var aid in aids)
            {
                aid.FetchPlayerRefs(rb, gc);
            }
        } 
    }
}
