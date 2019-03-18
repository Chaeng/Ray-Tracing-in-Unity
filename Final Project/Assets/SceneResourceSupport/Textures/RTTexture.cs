using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTexture : MonoBehaviour
{
    [SerializeField] private string m_name = "";

    // support for image
    [SerializeField] private int m_isColor = 0;

    // support for checker pattern
    [SerializeField] private int m_isChecker = 0;
    [SerializeField] private int m_uRepeat = 0;
    [SerializeField] private int m_vRepeat = 0;
    [SerializeField] private Vector3 m_color1 = Vector3.zero;
    [SerializeField] private Vector3 m_color2 = Vector3.zero;

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

            // support for checker pattern
            isChecker = m_isChecker,
            uRepeat = m_uRepeat, vRepeat = m_vRepeat,
            color1 = m_color1, color2 = m_color2
        };
    }
}
