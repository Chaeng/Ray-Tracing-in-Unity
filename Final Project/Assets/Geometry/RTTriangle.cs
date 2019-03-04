using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTriangle : RTGeometry
{
    /// <summary>
    /// The vertices of the triangle relative to the game object center
    /// </summary>
    [SerializeField] private Vector3 m_vertices0 = Vector3.zero;
    [SerializeField] private Vector3 m_vertices1 = Vector3.zero;
    [SerializeField] private Vector3 m_vertices2 = Vector3.zero;
    [SerializeField] private bool m_isDoubleSide = true;

    private Vector3 m_cachedVert0 = Vector3.zero;
    private Vector3 m_cachedVert1 = Vector3.zero;
    private Vector3 m_cachedVert2 = Vector3.zero;
    private Vector3 m_worldVert0 = Vector3.zero;
    private Vector3 m_worldVert1 = Vector3.zero;
    private Vector3 m_worldVert2 = Vector3.zero;
    private Vector3 m_cachedNormal;
    private float m_cachedPlaneD;
    private float m_cachedArea;


    public override RTGeometryType GetGeometryType()
    {
        return RTGeometryType.Triangle;
    }


    public RTTriangle_t GetGeometry()
    {
        if (IsAnyChangesOnVertices())
        {
            UpdateTriangle();
        }

        return new RTTriangle_t
        {
            vert0 = m_worldVert0,
            vert1 = m_worldVert1,
            vert2 = m_worldVert2,
            normal = m_cachedNormal,
            planeD = m_cachedPlaneD,
            area = m_cachedArea,
            isDoubleSide = m_isDoubleSide ? 1 : 0
        };
    }


    private bool IsAnyChangesOnVertices()
    {
        var _hasChanged = false;

        if (m_vertices0 != m_cachedVert0)
        {
            _hasChanged = true;
            m_cachedVert0 = m_vertices0;
        }
        
        if (m_vertices1 != m_cachedVert1)
        {
            _hasChanged = true;
            m_cachedVert1 = m_vertices1;
        }
        
        if (m_vertices2 != m_cachedVert2)
        {
            _hasChanged = true;
            m_cachedVert2 = m_vertices2;
        }

        return _hasChanged;
    }

    
    private void UpdateTriangle()
    {
        m_worldVert0 = m_vertices0 + transform.position;
        m_worldVert1 = m_vertices1 + transform.position;
        m_worldVert2 = m_vertices2 + transform.position;
        
        Vector3 cross = Vector3.Cross(m_worldVert1 - m_worldVert0, m_worldVert2 - m_worldVert0);
        m_cachedNormal = Vector3.Normalize(cross);
        m_cachedPlaneD = -1 * Vector3.Dot(m_cachedNormal, m_worldVert0);
        m_cachedArea = Vector3.Dot(m_cachedNormal, cross);
    }
    
}
