using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSphere))]
public class RTSphereRenderer : RTRenderer
{
    private void Awake()
    {
        m_geom = GetComponent<RTSphere>();    // Require Component Directive enforce that there must be a RTSphere component 
    }


    public RTSphere mySphere
    {
        get
        {
            if (m_geom == null)
            {
                m_geom = GetComponent<RTSphere>();
            }

            return (RTSphere)m_geom;
        }
    }


    public float[] GetGeometry()
    {
        if (mySphere == null)
        {
            return new float[4]{0, 0, 0, 0};
        }

        return mySphere.GetGeometry();
    }
}
