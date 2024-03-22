using UnityEngine;

public abstract class EnviromentalAid : MonoBehaviour, INeedPlayerRefs
{
    protected Rigidbody playerRB;
    protected MeshRenderer padRenderer;

    public Color unactivatedCol = Color.white;
    public Color activatedCol = Color.red;

    protected bool isActivated;

    protected virtual void Start()
    {
        padRenderer = GetComponent<MeshRenderer>();
        padRenderer.material.color = unactivatedCol;
    }

    public void FetchPlayerRefs(Rigidbody rb, GrindController gc = null)
    {
        playerRB = rb;
        OnFetchPlayerRefs(gc);
    }

    protected abstract void OnFetchPlayerRefs(GrindController gc);

    protected void ActivateVFX()
    {
        padRenderer.material.color = activatedCol;
        isActivated = true;
    }

    protected void DeactivateVFX()
    {
        padRenderer.material.color = unactivatedCol;
        isActivated = false;
    }
}
