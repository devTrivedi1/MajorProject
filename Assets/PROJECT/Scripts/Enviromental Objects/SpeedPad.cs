using UnityEngine;

public class SpeedPad : EnviromentalAid
{
    [SerializeField]
    private float groundspeedBoost = 10f;
    [SerializeField]
    private float railspeedBoost;

    private float OG_railSpeed;
    private GrindController GC;

    protected override void OnFetchPlayerRefs(GrindController gc)
    {
        GC = gc;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerRB != null && !isActivated)
        {
            isActivated = true; 

            if (GC != null && GC.isGrinding)
            {
                OG_railSpeed = GC.normalGrindSpeed;
                GC.normalGrindSpeed += railspeedBoost;
            }

            ActivateVFX();
            Vector3 boostForce = playerRB.transform.forward * groundspeedBoost;
            playerRB.AddForce(boostForce, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GC != null && GC.isGrinding)
        {
            GC.normalGrindSpeed = OG_railSpeed;
            isActivated = false;
        }
        DeactivateVFX();
    }
}
