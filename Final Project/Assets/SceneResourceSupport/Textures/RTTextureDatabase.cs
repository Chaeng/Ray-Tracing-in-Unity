using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTextureDatabase : MonoBehaviour
{
    private RTTexture[] m_textures = null;

    /// <summary>
    /// This returns texture type struct for renderer
    /// </summary>
    /// <returns>texture type struct</returns>
    public List<RTTexture> GetTextures()
    {
        if (m_textures == null)
        {
            m_textures = GetComponents<RTTexture>();
        }

        return m_textures == null
            ? null
            : new List<RTTexture>(m_textures);
    }
}
