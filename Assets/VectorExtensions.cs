using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 SetX(this Vector3 v, float x)
    {
        v = new Vector3(x, v.y, v.z);
        return v;
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        v = new Vector3(v.x, y, v.z);
        return v;
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v = new Vector3(v.x, v.y, z);
        return v;
    }


}
