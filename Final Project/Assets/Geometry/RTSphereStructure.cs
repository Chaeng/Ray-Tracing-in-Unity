
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTSphere_t
{
    public int id;
    public Vector3 center;
    public float radius;

    public static int GetSize()
    {
        return sizeof(int) + sizeof(float) * 3 + sizeof(float);
    }
}
