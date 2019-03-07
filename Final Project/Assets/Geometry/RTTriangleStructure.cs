
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTTriangle_t
{
    public Vector3 vert0;
    public Vector3 vert1;
    public Vector3 vert2;
    public Vector3 normal;
    public float planeD;
    public float area;
    public int isDoubleSide;
    public int geometryIndex;
    public int materialIndex;
    
    public static int GetSize()
    {
        return Vector3Extension.SizeOf() * 4
            + sizeof(float) * 2
            + sizeof(int) * 3;    // (vertices: 3 * 3 = 9) + (normal: 3) + (planeD: 1) + (area: 1)
    }
}
