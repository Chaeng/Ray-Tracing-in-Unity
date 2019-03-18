using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTexture : MonoBehaviour
{
    [SerializeField] private string m_name = "";

    // support for image
    [SerializeField] private int m_isColor = 0;
    [SerializeField] private Texture m_texture;
    private int m_imageIndex = -1;

    // support for checker pattern
    [SerializeField] private int m_isChecker = 0;
    [SerializeField] private int m_uRepeat = 0;
    [SerializeField] private int m_vRepeat = 0;
    [SerializeField] private Vector3 m_color1 = Vector3.zero;
    [SerializeField] private Vector3 m_color2 = Vector3.zero;


    private void Awake()
    {
        if (m_texture == null)
        {
            m_texture = new Texture2D(2, 2);
        }
    }

    public string GetName()
    {
        return m_name;
    }

    public RTTexture_t GetTexture()
    {
        return new RTTexture_t 
        {
            // support for image
            isColor = m_isColor,
            imageIndex = m_imageIndex,

            // support for checker pattern
            isChecker = m_isChecker,
            uRepeat = m_uRepeat, vRepeat = m_vRepeat,
            color1 = m_color1, color2 = m_color2
        };
    }

    public Texture GetImage()
    {
        return m_texture;
    }

    public void SetImageIndex(int index)
    {
        m_imageIndex = index;
    }
}
