using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RayTracingRenderPipeline
{
    private int m_shadowMapRes = 64;

    private RenderTexture m_shadowMap;

    private Texture2DArray m_shadowMapList;

    private List<ShadowUtility_t> m_shadowUtility;


    private void ShadowMapPass(RTLightStructureSpot_t spot,
                               int index,
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

        Vector3 lightPosition = spot.position;
        Vector3 direction = spot.direction;

        Vector3 center = lightPosition + (5 * -direction);
        Vector3 up = new Vector3(0, 1, 0);
        if (Mathf.Abs(Vector3.Dot(up, direction)) > 0.99999)
        {
            up = new Vector3(1, 0, 0);
        }

        Vector3 W = Vector3.Cross(up, direction);
        Vector3 U = Vector3.Cross(W, direction);
        U.Normalize();
        W.Normalize();

        float imageSize = 5 * Mathf.Tan(spot.coneAngle / 2);
        imageSize = imageSize * 2;
        float pixelSize = imageSize / m_shadowMapRes;

        Vector3 pref = center - U * (imageSize / 2) - W * (imageSize / 2);

        if (m_shadowUtility == null)
        {
            m_shadowUtility = new List<ShadowUtility_t>();
        }

        ShadowUtility_t temp = new ShadowUtility_t();

        temp.U = U;
        temp.W = W;
        temp.Pref = pref;
        temp.PixelSize = pixelSize;
        m_shadowUtility.Add(temp);


        int kIndex = m_shadowMapShader.FindKernel("ShadowMap");

        m_shadowMapShader.SetFloat("_PixelSize", pixelSize);
        m_shadowMapShader.SetFloats("_SpotLightPos", lightPosition.ToArray());
        m_shadowMapShader.SetFloats("_UnitU", U.ToArray());
        m_shadowMapShader.SetFloats("_UnitW", W.ToArray());
        m_shadowMapShader.SetFloats("_PrefPosition", pref.ToArray());

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

        Graphics.CopyTexture(m_shadowMap, 0, 0, m_shadowMapList, index, 0); // index is the index of the texture

    }
}
