﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RTRenderer : MonoBehaviour
{
    //[SerializeField] protected int m_materialIndex = -1;
    // [SerializeField] protected int m_materialName = -1;  // TODO

    protected RTGeometry m_geom = null;
    private RTMaterialDatabase m_materialDatabase = null;

    /// <summary>
    /// Awake is used to initialize any variables or game state before
    /// the game starts
    /// </summary>
    private void Awake()
    {
        GameObject[] roots = SceneManager.GetActiveScene()
            .GetRootGameObjects();

        foreach (var root in roots)
        {
            RTMaterialDatabase[] materialDatabases = root
                .GetComponentsInChildren<RTMaterialDatabase>();

            if(null != materialDatabases)
            {
                m_materialDatabase = materialDatabases[0];
            }
        }
    }

    /// <summary>
    /// This returns geometry type enum for renderer
    /// </summary>
    /// <returns>geometry type.</returns>
    public RTGeometryType GeometryType
    {
        get
        {
            if (m_geom == null)
            {
                return RTGeometryType.Nothing;
            }

            return m_geom.GetGeometryType();
        }
    }

    /// <summary>
    /// This returns material type struct for renderer
    /// </summary>
    /// <returns>material type struct</returns>
    public RTMaterial Material
    {
        get
        {
            return new RTMaterial();
            /*
            if (m_materialDatabase == null)
            {
                return new RTMaterial();
            }

            if (m_materialIndex < 0
                || m_materialIndex > m_materialDatabase.Materials.Length - 1)
            {
                return new RTMaterial();
            }

            return m_materialDatabase.Materials[m_materialIndex];
            */
        }
    }
}
