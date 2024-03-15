using UnityEngine;

public interface IResettable
{
    Transform transform {  get; }
    GameObject gameObject { get; }
}