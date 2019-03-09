using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterialDatabase : MonoBehaviour
{
    private RTMaterial[] m_materials = null;

    private void Awake()
    {
        m_materials = GetComponents<RTMaterial>();
    }

    /// <summary>
    /// This returns material type struct for renderer
    /// </summary>
    /// <returns>material type struct</returns>
    public RTMaterial[] Materials
    {
        get
        {
            return m_materials;
        }
    }
}
