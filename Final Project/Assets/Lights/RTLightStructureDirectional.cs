using UnityEngine;

/// <summary>
/// This C# struct corresponding to RTDirectionLight.compute
/// </summary>
public struct RTLightStructureDirectional_t
{
    public Vector3 color;
    public Vector3 direction;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 2;
    }
}