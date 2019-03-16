using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTMesh))]
public class RTMeshRenderer : RTRenderer
{
    public RTMesh myMesh
    {
        get
        {
            if (m_geom == null)
            {
                m_geom = GetComponent<RTMesh>();
            }

            return (RTMesh)m_geom;
        }
    }



    public List<RTTriangle_t> GetGeometry()
    {
        if (myMesh == null)
        {
            return null;
        }

        return myMesh.GetGeometry();
    }

}
