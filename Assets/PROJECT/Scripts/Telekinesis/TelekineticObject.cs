using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    Rigidbody rb;
    public Rigidbody Rb => rb;
    public bool manipulable { get; private set; } = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        manipulable = true;
    }

    public IEnumerator StopManipulation(float timer)
    {
        manipulable = false;
        float time = 0;
        while (time < timer)
        {
            time += Time.deltaTime;
            yield return null;
        }
        manipulable = true;
    }
}