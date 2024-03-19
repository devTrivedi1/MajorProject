using System.Linq;
using UnityEngine;

public class EnviromentalAidManager : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;
    private GrindController gc;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody>();
        gc = FindObjectOfType<GrindController>();
        INeedPlayerRefs[] aids = FindObjectsOfType<MonoBehaviour>().OfType<INeedPlayerRefs>().ToArray();
        foreach (var aid in aids)
        {
            aid.FetchPlayerRefs(rb);
            if (gc != null)
            {
                aid.FetchPlayerRefs(rb, gc);
            }
        }
    }
}
