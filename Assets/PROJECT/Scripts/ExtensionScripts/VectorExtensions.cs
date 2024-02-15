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

    public static Vector3 SetXAndY(this Vector3 v, float x, float y)
    {
        v = new Vector3(x, y, v.z);
        return v;
    }

    public static Vector3 SetXAndZ(this Vector3 v, float x, float z)
    {
        v = new Vector3(x, v.y, z);
        return v;
    }

    public static Vector3 SetYAndZ(this Vector3 v, float y, float z)
    {
        v = new Vector3(v.x, y, z);
        return v;
    }

    public static Vector3 Multiply(this Vector3 v, Vector3 other)
    {
        v = new Vector3(v.x * other.x, v.y * other.y, v.z * other.z);
        return v;
    }

    public static Vector3 Divide(this Vector3 v, Vector3 other)
    {
        v = new Vector3(v.x / other.x, v.y / other.y, v.z / other.z);
        return v;
    }
}
