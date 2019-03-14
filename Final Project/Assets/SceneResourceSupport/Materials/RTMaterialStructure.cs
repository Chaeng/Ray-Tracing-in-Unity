using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RTMaterial_t
{
    public int id;

    // classic Ambient, Diffuse, Specular
    public Vector3 ka, ks, kd;
    public float n;

    // support for transparency and refractiveIndex
    public float transparency;
    public float refractiveIndex; 

    // support for reflection
    public float reflectivity;
    
    // support for normal and positional map
    public Vector3 normal, position;

    public static int GetSize()
    {
        return sizeof(int)
            + Vector3Extension.SizeOf() * 5
            + sizeof(float) * 4;
    }
}
