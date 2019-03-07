﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RayTracingRenderPipeline
{
    private int m_shadowMapWidth = 2;
    private int m_shadowMapHeight = 2;

    private RenderTexture m_shadowMap;


    private void ShadowMapPass(Vector3 spotLightDirection, Vector3 spotLightPosition, int numOfSphere, ComputeBuffer spheres, int numofTriangle, ComputeBuffer triangles)
    {
        if(m_shadowMap == null)
        {
            m_shadowMap = new RenderTexture(m_shadowMapWidth, m_shadowMapHeight, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_shadowMap.enableRandomWrite = true;
            m_shadowMap.Create();
        }

        m_shadowMapShader.SetFloats("_SpotLightDir", spotLightDirection.ToArray());
        m_shadowMapShader.SetFloats("_SpotLightPos", spotLightPosition.ToArray());
        m_shadowMapShader.SetInt("_NumOfSpheres", numOfSphere);
        m_shadowMapShader.SetBuffer(0, "_Spheres", spheres);
        m_shadowMapShader.SetInt("_NumOfTriangles", numofTriangle);
        m_shadowMapShader.SetBuffer(0, "_Triangles", triangles);

        m_shadowMapShader.SetTexture(0, "Result", m_shadowMap);
        int threadGroupsX = Mathf.CeilToInt(m_shadowMapWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(m_shadowMapHeight / 8.0f);
        if (threadGroupsX > 0 && threadGroupsY > 0
        ) // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        {
            m_shadowMapShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }


        /**
        The below lines of code are for Kai to debug
         */

        RenderTexture.active = m_shadowMap;
        Texture2D tempResult = new Texture2D(m_shadowMap.width, m_shadowMap.height, TextureFormat.ARGB32, false, false);
        tempResult.ReadPixels(new Rect(0, 0, m_shadowMap.width, m_shadowMap.height), 0, 0);
        tempResult.Apply();

        // You should be able to get the result of each pixel form here
        Vector4 result00 = tempResult.GetPixel(0, 0);
    }
}
