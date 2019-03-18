using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterial : MonoBehaviour
{   
    [SerializeField] private string m_name = "";
    
    // classic Ambient, Diffuse, Specular
    [SerializeField] private Vector3 m_ka = Vector3.zero;
    [SerializeField] private Vector3 m_ks = Vector3.zero;
    [SerializeField] private Vector3 m_kd = Vector3.zero;
    [SerializeField] private float m_n = 0;

    // support for transparency and refractiveIndex
    [SerializeField] private float m_transparency = 0;
    [SerializeField] private float m_refractiveIndex = 0; 

    // support for reflection
    [SerializeField] private float m_reflectivity = 0;
    
    // support for normal and positional map
    [SerializeField] private Vector3 m_normal = Vector3.zero;
    [SerializeField] private Vector3 m_position = Vector3.zero;
    
    // support for textures
    [SerializeField] private string m_textureNameKa = "";
    [SerializeField] private string m_textureNameKd = "";
    [SerializeField] private string m_textureNameKs = "";

    private int m_textureIndexKa = -1;
    private int m_textureIndexKs = -1;
    private int m_textureIndexKd = -1;

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
            normal = m_normal, position = m_position,

            // support for textures
            textureIndexKa =  m_textureIndexKa,
            textureIndexKd = m_textureIndexKd,
            textureIndexKs = m_textureIndexKs,
        };
    }

    public string GetTextureNameKa()
    {
        return m_textureNameKa;
    }

    public string GetTextureNameKd()
    {
        return m_textureNameKd;
    }

    public string GetTextureNameKs()
    {
        return m_textureNameKs;
    }

    public void SetTextureIndexKa(int index)
    {
        m_textureIndexKa = index;
    }

    public void SetTextureIndexKd(int index)
    {
        m_textureIndexKd = index;
    }

    public void SetTextureIndexKs(int index)
    {
        m_textureIndexKs = index;
    }
}
