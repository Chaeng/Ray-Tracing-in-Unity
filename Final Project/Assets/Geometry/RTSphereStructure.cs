
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTSphere_t
{
    public int id;  // geometryIndex
    public Vector3 center;
    public float radius;
    public int materialIndex;

    public static int GetSize()
    {
        return Vector3Extension.SizeOf()
            + sizeof(int) * 2
            + sizeof(float);
    }
}
