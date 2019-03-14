using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTTexture : MonoBehaviour
{
    [SerializeField] private string m_name;

    // support for checker
    [SerializeField] private int m_isChecker;
    [SerializeField] private int m_uRepeat, m_vRepeat;
    [SerializeField] private Vector3 m_color1, m_color2;

    public string GetName()
    {
        return m_name;
    }

    public RTTexture_t GetTexture()
    {
        return new RTTexture_t 
        {
            // support for checker
            isChecker = m_isChecker,
            uRepeat = m_uRepeat, vRepeat = m_vRepeat,
            color1 = m_color1, color2 = m_color2
        };
    }
}
