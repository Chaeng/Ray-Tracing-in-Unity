using UnityEngine;

/// <summary>
/// This structure provides material info
/// </summary>
public struct RTMaterial_t
{   
    // classic Ambient, Diffuse, Specular
    public Vector3 ka, ks, kd;
    public Vector3 n;

    // support for transparency and refractiveIndex
    public float transparency;
    public float refractiveIndex; 

    // support for reflection
    public float reflectivity;
    
    // support for normal and positional map
    public Vector3 normal, position;

    public static int GetSize()
    {
        return Vector3Extension.SizeOf() * 6
            + sizeof(float) * 3;
    }
}
