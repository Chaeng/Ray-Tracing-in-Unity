using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterial : MonoBehaviour
{   
    [SerializeField] private string m_name;
    
    // classic Ambient, Diffuse, Specular
    [SerializeField] private Vector3 m_ka, m_ks, m_kd;
    [SerializeField] private float m_n;

    // support for transparency and refractiveIndex
    [SerializeField] private float m_transparency;
    [SerializeField] private float m_refractiveIndex; 

    // support for reflection
    [SerializeField] private float m_reflectivity;
    
    // support for normal and positional map
    [SerializeField] private Vector3 m_normal, m_position;

    public string GetName()
    {
        return m_name;
    }

    public RTMaterial_t GetMaterial()
    {
        return new RTMaterial_t 
        {
            // classic Ambient, Diffuse, Specular
            ka = m_ka, ks = m_ks, kd = m_kd,
            n = m_n,

            // support for transparency and refractiveIndex
            transparency = m_transparency,
            refractiveIndex = m_refractiveIndex,

            // support for reflection
            reflectivity = m_reflectivity,
            
            // support for normal and positional map
            normal = m_normal, position = m_position
        };
    }
}
