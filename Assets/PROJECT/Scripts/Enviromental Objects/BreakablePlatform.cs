using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BreakablePlatform : MonoBehaviour, IEnviromentalAids
{
    public float maxTime;
    private Rigidbody playerRB;
    private Rigidbody PadRB;
    private MeshRenderer padRenderer;
    private bool playerOnPad;
    private float timer = 0.0f;

    public Color startColor = Color.white; 
    public Color endColor = Color.red; 
    private void Start()
    {
        PadRB = GetComponent<Rigidbody>();
        PadRB.useGravity = false;
        PadRB.isKinematic = true;

         padRenderer = GetComponent<MeshRenderer>();
         padRenderer.material.color = startColor;
    }

    private void Update()
    {
        if (playerOnPad)
        {
            timer += Time.deltaTime;
            if (timer >= maxTime)
            {
                PadRB.useGravity = true;
                PadRB.isKinematic = false;
            }
            else
            {
                float ratio = 1f - (timer / maxTime);
                padRenderer.material.color = Color.Lerp(endColor, startColor, ratio);
            }
        }
    }

    public void SetTargetRigidbody(Rigidbody rb)
    {
        playerRB = rb;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerRB != null)
        {
            playerOnPad = true;
        }
    }
}
