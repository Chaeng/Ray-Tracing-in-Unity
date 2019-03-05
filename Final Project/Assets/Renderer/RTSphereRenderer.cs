using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSphere))]
public class RTSphereRenderer : RTRenderer
{
    /// <summary>
    /// Awake is used to initialize any variables or game state before
    /// the game starts
    /// </summary>
    private void Awake()
    {
        m_geom = GetComponent<RTSphere>();    // Require Component Directive enforce that there must be a RTSphere component 
    }

    /// <summary>
    /// Property to get attached RTSphere component
    /// </summary>
    /// <returns>RTSphere component</returns>
    private RTSphere mySphere
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

    /// <summary>
    /// Get RTSphere_t geometry for attached RTSphere component
    /// </summary>
    /// <returns>RTSphere_t geometry</returns>
    public RTSphere_t GetGeometry()
    {
        if (mySphere == null)
        {
            return new RTSphere_t()
            {
                center = Vector3.zero,
                radius = 0
            };
        }

        return mySphere.GetGeometry();
    }
}
