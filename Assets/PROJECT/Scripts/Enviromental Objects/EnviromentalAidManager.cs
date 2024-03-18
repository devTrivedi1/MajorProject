using System.Linq;
using UnityEngine;

public class EnviromentalAidManager : MonoBehaviour
{
    public GameObject player;
    public Rigidbody rb;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody>();
        IEnviromentalAids[] aids = FindObjectsOfType<MonoBehaviour>().OfType<IEnviromentalAids>().ToArray();
        foreach (var aid in aids)
        {
            aid.SetTargetRigidbody(rb);
        }
    }
}
