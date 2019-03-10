using UnityEngine;

/// <summary>
/// This C# struct corresponding to RTPointLight.compute
/// </summary>
public struct RTLightStructureSpot_t
{
    public Vector3 color;
    public Vector3 position;
    public Vector3 direction;
    public float coneAngle;
    public float cosConeAngle;
    public int shadowMap;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 3 + sizeof(float) * 2 + sizeof(int);
    }
}