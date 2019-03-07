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
    public Vector3 U;
    public Vector3 W;
    public Vector3 Pref;
    public float PixelSize;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 6 + sizeof(float) * 2;
    }

    public void SetU(Vector3 input)
    {
        U = input;
    }

    public void SetW(Vector3 input)
    {
        W = input;
    }

    public void SetPref(Vector3 input)
    {
        Pref = input;
    }

    public void SetPixelSize(float input)
    {
        PixelSize = input;
    }
}