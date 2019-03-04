using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTTriangle))]
public class RTTriangleRenderer : RTRenderer
{
    private void Awake()
    {
        m_geom = GetComponent<RTTriangle>();    // Require Component Directive enforce that there must be a RTSphere component 
    }
    
    public RTTriangle myTriangle
    {
        get
        {
            if (m_geom == null)
            {
                m_geom = GetComponent<RTTriangle>();
            }

            return (RTTriangle)m_geom;
        }
    }
    
    public RTTriangle_t GetGeometry()
    {
        if (myTriangle == null)
        {
            return new RTTriangle_t()
            {
                
            };
        }

        return myTriangle.GetGeometry();
    }
}
