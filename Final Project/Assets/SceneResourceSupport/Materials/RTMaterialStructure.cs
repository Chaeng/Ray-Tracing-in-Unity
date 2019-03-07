using UnityEngine;

public struct RTMaterial_t
{   
    // TODO: Move to common reference utility class
    static readonly int SIZEOF_VECTOR3 = sizeof(float) * 3;

    // private string MaterialName;  // TODO
    
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
        return SIZEOF_VECTOR3 * 6
            + sizeof(float) * 3;
    }
}
