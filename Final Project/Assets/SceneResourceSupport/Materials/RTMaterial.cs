using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterial : MonoBehaviour
{   
    // [SerializeField] private string m_MaterialName;  // TODO
    
    // classic Ambient, Diffuse, Specular
    [SerializeField] private Vector3 m_ka, m_ks, m_kd;
    [SerializeField] Vector3 m_n;

    // support for transparency and refractiveIndex
    [SerializeField] float m_transparency;
    [SerializeField] float m_refractiveIndex; 

    // support for reflection
    [SerializeField] float m_reflectivity;
    
    // support for normal and positional map
    [SerializeField] Vector3 m_normal, m_position;
}
