using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    public Rigidbody rb { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}