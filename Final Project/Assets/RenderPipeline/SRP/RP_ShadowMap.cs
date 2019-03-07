using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RayTracingRenderPipeline
{
    private int m_shadowMapRes = 128;

    private RenderTexture m_shadowMap;

    private Texture2DArray m_shadowMapList;

    private void ShadowMapPass(int index,
                               int numOfSphere, 
                               ComputeBuffer spheres, 
                               int numofTriangle, 
                               ComputeBuffer triangles)
    {
        if(m_shadowMap == null)
        {
            m_shadowMap = new RenderTexture(m_shadowMapRes, m_shadowMapRes, 0,
                                            RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_shadowMap.enableRandomWrite = true;
            m_shadowMap.Create();
        }

        Vector3 lightPosition = m_spotLights[index].position;
        Vector3 direction = m_spotLights[index].direction;

        Vector3 center = lightPosition + (5 * direction);
        Vector3 up = new Vector3(0, 1, 0);
        if (Mathf.Abs(Vector3.Dot(up, direction)) > 0.99999)
        {
            up = new Vector3(1, 0, 0);
        }

        Vector3 W = Vector3.Cross(up, direction);
        Vector3 U = Vector3.Cross(W, direction);
        U.Normalize();
        W.Normalize();

        float imageSize = 5 * Mathf.Tan(m_spotLights[index].coneAngle / 2);
        imageSize = imageSize * 2;
        float pixelSize = imageSize / m_shadowMapRes;

        Vector3 pref = center - U * (imageSize / 2) - W * (imageSize / 2);

        m_spotLights[index].SetU(U);
        m_spotLights[index].SetW(W);
        m_spotLights[index].SetPref(pref);
        m_spotLights[index].SetPixelSize(pixelSize);

        int kIndex = m_shadowMapShader.FindKernel("ShadowMap");

        m_shadowMapShader.SetFloat("_PixelSize", pixelSize);
        m_shadowMapShader.SetFloats("_SpotLightPos", lightPosition.ToArray());
        m_shadowMapShader.SetFloats("_UnitU", U.ToArray());
        m_shadowMapShader.SetFloats("_UnitW", W.ToArray());
        m_shadowMapShader.SetInt("_NumOfSpheres", numOfSphere);
        m_shadowMapShader.SetBuffer(kIndex, "_Spheres", spheres);
        m_shadowMapShader.SetInt("_NumOfTriangles", numofTriangle);
        m_shadowMapShader.SetBuffer(kIndex, "_Triangles", triangles);
      
        m_shadowMapShader.SetTexture(kIndex, "_ShadowMap", m_shadowMap);
        int threadGroupsX = Mathf.CeilToInt(m_shadowMapRes / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(m_shadowMapRes / 8.0f);

        // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        if (threadGroupsX > 0 && threadGroupsY > 0) 
        {
            m_shadowMapShader.Dispatch(kIndex, threadGroupsX, threadGroupsY, 1);
        }

        Graphics.CopyTexture(m_shadowMap, 0, 0, m_shadowMapList, index, 0); // i is the index of the texture


        ///**
        //The below lines of code are for Kai to debug
        // */

        //RenderTexture.active = m_shadowMap;
        //Texture2D tempResult = new Texture2D(m_shadowMap.width, m_shadowMap.height, TextureFormat.ARGB32, false, false);
        //tempResult.ReadPixels(new Rect(0, 0, m_shadowMap.width, m_shadowMap.height), 0, 0);
        //tempResult.Apply();

        //// You should be able to get the result of each pixel form here
        //Vector4 result00 = tempResult.GetPixel(0, 0);
    }
}
