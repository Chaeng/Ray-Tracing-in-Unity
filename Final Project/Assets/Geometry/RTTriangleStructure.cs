
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTTriangle_t
{
    public int id;
    public Vector3 vert0;
    public Vector3 vert1;
    public Vector3 vert2;
    public Vector3 normal;
    public float planeD;
    public float area;
    public int isDoubleSide;
    
    public static int GetSize()
    {
        return sizeof(int) + sizeof(float) * 14 + sizeof(int);    // (vertices: 3 * 3 = 9) + (normal: 3) + (planeD: 1) + (area: 1)
    }
}
