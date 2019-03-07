
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTSphere_t
{
    public Vector3 center;
    public float radius;
    public int geometryIndex;
    public int materialIndex;

    public static int GetSize()
    {
        return Vector3Extension.SizeOf()
            + sizeof(float)
            + sizeof(int) * 2;    // (vertices: 3 * 3 = 9) + (normal: 3) + (planeD: 1) + (area: 1)
    }
}
