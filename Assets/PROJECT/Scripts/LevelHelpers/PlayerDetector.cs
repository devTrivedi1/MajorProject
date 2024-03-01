using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] SplineComputer spline;

    private void OnValidate()
    {
        if (spline == null) { spline = gameObject.GetComponent<SplineComputer>(); }
        TryGetComponent(out MeshCollider splineCollider);
        if (splineCollider == null) { splineCollider = gameObject.AddComponent<MeshCollider>(); }
        splineCollider.sharedMesh = spline.GetComponent<MeshFilter>().sharedMesh;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GrindController grindController = collision.gameObject.GetComponent<GrindController>();
        Jump jumpController = collision.gameObject.GetComponent<Jump>();
        if(grindController == null) { return; }
        grindController.GoGrindOnThoseRails(spline);
        jumpController.SetJumpState(JumpState.Grounded);

    }
}
