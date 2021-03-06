﻿using System.Collections;
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
    [SerializeField] private Vector2 m_verticesUV0 = Vector3.zero;
    [SerializeField] private Vector2 m_verticesUV1 = Vector3.zero;
    [SerializeField] private Vector2 m_verticesUV2 = Vector3.zero;
    [SerializeField] private bool m_isDoubleSide = true;
    [SerializeField] private string m_materialName = "";

    private int m_materialIndex = -1;


    private Vector4 m_worldVert0 = Vector4.zero;
    private Vector4 m_worldVert1 = Vector4.zero;
    private Vector4 m_worldVert2 = Vector4.zero;
    private Vector3 m_cachedNormal;
    private float m_cachedPlaneD;
    private float m_cachedArea;
    private Vector3 m_cahcedPosition = Vector3.zero;


    public override RTGeometryType GetGeometryType()
    {
        return RTGeometryType.Triangle;
    }

    public string GetMaterialName()
    {
        return m_materialName;
    }

    public void SetMaterialIndex(int index)
    {
        m_materialIndex = index;
    }

    public RTTriangle_t GetGeometry()
    {
        UpdateTriangle();

        return new RTTriangle_t
        {
            vert0 = m_worldVert0,
            vert1 = m_worldVert1,
            vert2 = m_worldVert2,
            normal = m_cachedNormal,
            planeD = m_cachedPlaneD,
            area = m_cachedArea,
            isDoubleSide = m_isDoubleSide ? 1 : 0,
            materialIndex = m_materialIndex,
            vertUV0 = m_verticesUV0,
            vertUV1 = m_verticesUV1,
            vertUV2 = m_verticesUV2
        };
    }

    
    private void UpdateTriangle()
    {
//        m_worldVert0 = (transform.localRotation * m_vertices0) + transform.localPosition;
//        m_worldVert1 = (transform.localRotation * m_vertices1) + transform.localPosition;
//        m_worldVert2 = (transform.localRotation * m_vertices2) + transform.localPosition;

        Matrix4x4 localToWorldMat = transform.localToWorldMatrix;
        m_worldVert0 = localToWorldMat.MultiplyPoint(m_vertices0);
        m_worldVert1 = localToWorldMat.MultiplyPoint(m_vertices1);
        m_worldVert2 = localToWorldMat.MultiplyPoint(m_vertices2);
        
        Vector3 cross = Vector3.Cross(m_worldVert1 - m_worldVert0, m_worldVert2 - m_worldVert0);
        m_cachedNormal = Vector3.Normalize(cross);
        m_cachedPlaneD = -1 * Vector3.Dot(m_cachedNormal, m_worldVert0);
        m_cachedArea = Vector3.Dot(m_cachedNormal, cross);
    }


    private Vector3 GetScaledVector(Vector3 v, Vector3 scale)
    {
        return new Vector3(v.x * scale.x, v.y + scale.y / 2f, v.z * scale.z);
    }
    
}
