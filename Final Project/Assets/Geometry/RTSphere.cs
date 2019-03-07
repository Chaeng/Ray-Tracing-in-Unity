using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSphere : RTGeometry
{
    [SerializeField] private float m_radius = 1;
    [SerializeField] private int m_materialIndex = -1;


    public override RTGeometryType GetGeometryType()
    {
        return RTGeometryType.Sphere;
    }


    public RTSphere_t GetGeometry()
    {
        var _center = transform.position;
        
        
        return new RTSphere_t 
        {
            center = _center,
            radius = m_radius,
            materialIndex = m_materialIndex
        };
    }
}
