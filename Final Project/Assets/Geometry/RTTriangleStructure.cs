
using UnityEngine;

/// <summary>
/// This structure matches with the RTSphere_t structure defined in RTSphere.compute
/// </summary>
public struct RTTriangle_t
{
    public int id;  // geometryIndex
    public Vector3 vert0;
    public Vector3 vert1;
    public Vector3 vert2;
    public Vector2 vertUV0;
    public Vector2 vertUV1;
    public Vector2 vertUV2;
    public Vector3 normal;
    public float planeD;
    public float area;
    public int isDoubleSide;
    public int materialIndex;
    // public int textureIndex;
    
    public static int GetSize()
    {
        return Vector3Extension.SizeOf() * 4
            + Vector2Extension.SizeOf() * 3
            // + sizeof(int) * 4
            + sizeof(int) * 3
            + sizeof(float) * 2;
    }
}
