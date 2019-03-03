using UnityEngine;

/// <summary>
/// This C# struct corresponding to RTPointLight.compute
/// </summary>
public struct RTLightStructurePoint_t
{
    public Vector3 color;
    public Vector3 direction;
    public Vector3 position;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 3;
    }
}