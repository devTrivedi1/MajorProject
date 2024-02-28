using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    Rigidbody rb;
    public Rigidbody Rb => rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}