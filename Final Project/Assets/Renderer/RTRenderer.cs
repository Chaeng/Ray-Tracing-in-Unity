using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class RTRenderer : MonoBehaviour
{
    protected RTGeometry m_geom = null;


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
    
    
    
}
