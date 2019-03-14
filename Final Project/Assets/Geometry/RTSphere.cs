using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSphere : RTGeometry
{
    [SerializeField] private float m_radius = 1;
    [SerializeField] private string m_materialName;

    private int m_materialIndex = -1;

    public override RTGeometryType GetGeometryType()
    {
        return RTGeometryType.Sphere;
    }

    public string GetMaterialName()
    {
        return m_materialName;
    }

    public void SetMaterialIndex(int index)
    {
        m_materialIndex = index;
    }

    public RTSphere_t GetGeometry()
    {
        var _center = transform.position;
        
        
        return new RTSphere_t 
        {
            center = _center,
            radius = m_radius
        };
    }
}
