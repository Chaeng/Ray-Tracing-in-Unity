using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering; // Since SRP is an experimental feature, we have to import it using this namespace
using UnityEngine.SceneManagement;

public partial class RayTracingRenderPipeline
{
    private readonly static string s_bufferName = "Ray Tracing Render Camera";

    private ComputeShader m_mainShader; // The compute shader we are going to write our ray tracing program on
    private ComputeShader m_shadowMapShader;

    private RenderTexture m_target; // The texture to hold the ray tracing result from the compute shader

    private RenderTexture m_depthMap; // The texture to hold the shadow depth map

    private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings      

    private RenderPipelineConfigObject m_config;

    private List<RTLightStructureDirectional_t> m_directionalLights;
    private List<RTLightStructurePoint_t> m_pointLights;
    private List<RTLightStructureSpot_t> m_spotLights;

    private List<RTSphere_t> m_sphereGeom;
    private List<RTTriangle_t> m_triangleGeom;

    private void LoadBufferWithSpheres(ref ComputeBuffer sphereBuffer)
    {
        if (m_sphereGeom.Count > 0)
        {
            sphereBuffer = new ComputeBuffer(m_sphereGeom.Count, sizeof(float)*4);
            sphereBuffer.SetData(m_sphereGeom);
        }
        else
        {
            sphereBuffer = new ComputeBuffer(1, sizeof(float) * 4);
        }
    }


    private void LoadBufferWithTriangles(ref ComputeBuffer triangleBuffer)
    {
        if (m_triangleGeom.Count > 0)
        {
            triangleBuffer = new ComputeBuffer(m_triangleGeom.Count, RTTriangle_t.GetSize());
            triangleBuffer.SetData(m_triangleGeom);
        }
        else
        {
            triangleBuffer = new ComputeBuffer(1, RTTriangle_t.GetSize());
        }
    }
}
