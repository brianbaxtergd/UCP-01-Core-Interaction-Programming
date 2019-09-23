using UnityEngine;


public static class Vector3Extensions
{
    static public Vector3 ComponentDivide(this Vector3 v0, Vector3 v1)
    {
        Vector3 vRes = v0;

        // Avoid divide by zero errors.
        if (v1.x != 0)
        {
            vRes.x = v0.x / v1.x;
        }
        if (v1.y != 0)
        {
            vRes.y = v0.y / v1.y;
        }
        if (v1.z != 0)
        {
            vRes.z = v0.z / v1.z;
        }

        return vRes;
    }
}
