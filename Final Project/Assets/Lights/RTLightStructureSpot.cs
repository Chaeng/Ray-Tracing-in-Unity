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
    public float cosFullIlluminous;
    public float penumbraDecay;
    public int enableShadowMap;
    public int ShadowFilterRes;
    public float range;
    public float baseD;
    public float fogDensity;
    public int enablefog;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 3 + sizeof(float) * 7 + sizeof(int) * 3;
    }
}
