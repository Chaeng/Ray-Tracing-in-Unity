using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterialDatabase : MonoBehaviour
{
    private RTMaterial[] m_materials = null;

    /// <summary>
    /// This returns material type struct for renderer
    /// </summary>
    /// <returns>material type struct</returns>
    public List<RTMaterial> GetMaterials()
    {
        if (m_materials == null)
        {
            m_materials = GetComponents<RTMaterial>();
        }

        return m_materials == null
            ? null
            : new List<RTMaterial>(m_materials);
    }
}
