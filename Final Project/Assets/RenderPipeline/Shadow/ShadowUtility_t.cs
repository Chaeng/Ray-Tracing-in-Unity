using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This C# struct corresponding to RTPointLight.compute
/// </summary>
public struct ShadowUtility_t
{
    public Vector3 U;
    public Vector3 W;
    public Vector3 Pref;
    public float PixelSize;
    public int mapRes;

    public static int GetSize()
    {
        return sizeof(float) * 3 * 3 + sizeof(float) + sizeof(int);
    }
};