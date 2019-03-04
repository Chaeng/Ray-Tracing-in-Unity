using UnityEngine;

public struct RTMaterialType
{
    // classic Ambient, Diffuse, Specular
    Vector3 ka, ks, kd;
    Vector3 n;

    // support for transparency and refractiveIndex
    float transparency;
    float refractiveIndex; 

    // support for reflection
    float reflectivity;
    
    // support for normal and positional map
    Vector3 normal, position;
}

