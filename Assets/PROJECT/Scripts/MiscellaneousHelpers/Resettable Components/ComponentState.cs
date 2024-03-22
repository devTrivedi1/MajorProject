public abstract class ComponentState
{
    public abstract void CaptureState(IResettable resettable);

    public abstract void ResetState();
}