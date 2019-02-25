using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSphere : RTGeometry
{
    [SerializeField] private float m_radius = 1;


    public override RTGeometryType GetGeomtryType()
    {
        return RTGeometryType.Sphere;
    }


    public float[] GetGeometry()
    {
        var center = transform.position;
        
        return new float[4]
        {
            center.x,
            center.y,
            center.z,
            m_radius
        };
    }
}
