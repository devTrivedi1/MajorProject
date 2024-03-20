using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour, INeedPlayerRefs
{
    [SerializeField]
    private float groundspeedBoost = 10f;

    private Rigidbody playerRB;
    private MeshRenderer padRenderer;

    [SerializeField]
    private float railspeedBoost;

    [SerializeField]
    private float OG_railSpeed;

    private float forwardInfluence;
    private GrindController GC;

    public Color unactivatedCol = Color.white;
    public Color activatedCol = Color.red;
    private void Start()
    {
        padRenderer = GetComponent<MeshRenderer>();
        padRenderer.material.color = unactivatedCol;
    }

    public void FetchPlayerRefs(Rigidbody rb, GrindController gc)
    {
        playerRB = rb;
        GC = gc;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (playerRB != null)
        {
            //other.gameObject.TryGetComponent(out GrindController GC);

            if (GC != null && GC.isGrinding == true)
            {
                OG_railSpeed = GC.normalGrindSpeed;
                GC.normalGrindSpeed = GC.normalGrindSpeed + railspeedBoost;
                Debug.Log(OG_railSpeed);
            }

            padRenderer.material.color = activatedCol;
            Vector3 boostForce = playerRB.transform.forward * groundspeedBoost;
            playerRB.AddForce(boostForce, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent(out GrindController GC);
        if (GC.isGrinding == true)
        {
            GC.normalGrindSpeed = OG_railSpeed;
        }
        padRenderer.material.color = unactivatedCol;
    }
}
