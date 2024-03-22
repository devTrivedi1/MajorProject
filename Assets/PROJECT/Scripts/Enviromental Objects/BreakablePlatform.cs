using UnityEngine;

public class BreakablePlatform : EnviromentalAid
{
    public float maxTime;
    private Rigidbody PadRB;
    private bool playerOnPad;
    private float timer = 0.0f;

    public Color startColor = Color.white;
    public Color endColor = Color.red;

    protected override void Start()
    {
        base.Start(); 
        PadRB = GetComponent<Rigidbody>();
        PadRB.useGravity = false;
        PadRB.isKinematic = true;
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

    protected override void OnFetchPlayerRefs(GrindController gc)
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerRB.gameObject)
        {
            playerOnPad = true;
            ActivateVFX(); 
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerRB.gameObject)
        {
            playerOnPad = false;
            DeactivateVFX(); 
        }
    }
}

