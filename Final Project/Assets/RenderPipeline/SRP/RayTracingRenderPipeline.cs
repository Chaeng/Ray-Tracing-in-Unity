using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering; // Since SRP is an experimental feature, we have to import it using this namespace
using UnityEngine.SceneManagement;


public partial class RayTracingRenderPipeline : RenderPipeline
{

    // We batch the commands into a buffer to reduce the amount of sending commands to GPU
    // Reusing the command buffer object avoids continuous memory allocation
    private CommandBuffer m_buffer = new CommandBuffer
    {
        name = s_bufferName
    };

    /// <summary>
    /// Constructs the render pipeline for Unity.
    /// </summary>
    /// <param name="computeShader">Compute shader to use.</param>
    /// <param name="skybox">Skybox to use</param>
    public RayTracingRenderPipeline(ComputeShader mainShader, ComputeShader shadowMapShader, List<RenderPipelineConfigObject> allConfig)
    {
        m_mainShader = mainShader;

        m_shadowMapShader = shadowMapShader;

        m_allConfig = allConfig;
        m_config = m_allConfig[0];
    }


    /// <summary>
    /// This method get called by Unity every frame to draw on the screen. 
    /// </summary>
    /// <param name="renderContext">Render context.</param>
    /// <param name="cameras">All running cameras</param>
    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        // RenderPipeline.Render doesn't draw anything, but checks
        // whether the pipeline object is valid to use for rendering.
        // If not, it will raise an exception. We will override this
        // method and invoke the base implementation, to keep this check.
        base.Render(renderContext, cameras);

        var scene = SceneManager.GetActiveScene();
        ApplyRenderConfig(scene);
        if (m_config == null)
        {
            return;
        }

        ParseScene(scene);

        foreach (var camera in cameras)
        {
            RenderPerCamera(renderContext, camera);
        }
    }


    private void ApplyRenderConfig(Scene scene)
    {
        var sceneIndex = scene.buildIndex;

        if (m_allConfig.Count == 0)
        {
            return;
        }

        if (sceneIndex >= 0 && sceneIndex < m_allConfig.Count)
        {
            m_config = m_allConfig[sceneIndex];
            return;
        }

        // No matching scene index, use the first one
        m_config = m_allConfig[0];
    }

    /// <summary>
    /// This method responsible for rendering on per camera basis
    /// </summary>
    /// <param name="renderContext">Render context.</param>
    /// <param name="camera">Camera.</param>
    private void RenderPerCamera(ScriptableRenderContext renderContext, Camera camera)
    {
        InitRenderTexture();

        //renderContext.SetupCameraProperties(camera);    // This tells the renderer the camera position and orientation, projection type (perspective/orthographic)


        #region Clear Flag - Clear the previous frame

        CameraClearFlags
            clearFlags =
                camera.clearFlags; // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
        m_buffer.ClearRenderTarget(
            ((clearFlags & CameraClearFlags.Depth) != 0),
            ((clearFlags & CameraClearFlags.Color) != 0),
            camera.backgroundColor);

        #endregion

        // Begin Unity profiler sample for frame debugger
        m_buffer.BeginSample(s_bufferName);



        #region Geometry Preparation

        ComputeBuffer sphereBuffer = null;
        LoadBufferWithSpheres(ref sphereBuffer);
        ComputeBuffer triangleBuffer = LoadBufferWithTriangles();

        #endregion


        #region Global map buffer init

        //Shader.SetGlobalTexture("_ShadowMap", m_shadowMap);

        #endregion



        #region Shadow Map Pass

        //ShadowMapPass(Vector3.zero, Vector3.zero, m_sphereGeom.Count, sphereBuffer, m_triangleGeom.Count, triangleBuffer);

        #endregion



        #region Ray Tracing

        int kIndex = m_mainShader.FindKernel("CSMain");

        // Shadow Depth Map for Spot Light
        // m_mainShader.SetTextureFromGlobal(kIndex, "_DepthMap", "_ShadowMap");

        m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        m_mainShader.SetTexture(kIndex, "_SkyboxTexture", m_config.skybox);

        // Sphere

        m_mainShader.SetInt("_NumOfSpheres", m_sphereGeom.Count);
        m_mainShader.SetBuffer(kIndex, "_Spheres", sphereBuffer);

        // Triangle

        m_mainShader.SetInt("_NumOfTriangles", m_triangleGeom.Count);


        m_mainShader.SetBuffer(kIndex, "_Triangles", triangleBuffer);

        // Ambient Light

        m_mainShader.SetVector("_AmbientGlobal", m_config.ambitent);

        // Directional Lights

        m_mainShader.SetInt("_NumOfDirectionalLights", m_directionalLights.Count);

        ComputeBuffer dirLightBuf = null;
        if (m_directionalLights.Count > 0)
        {
            dirLightBuf = new ComputeBuffer(m_directionalLights.Count, RTLightStructureDirectional_t.GetSize());
            dirLightBuf.SetData(m_directionalLights);
        }
        else
        {
            dirLightBuf = new ComputeBuffer(1, 4); // Dummy
        }

        m_mainShader.SetBuffer(kIndex, "_DirectionalLights", dirLightBuf);

        // Point Lights

        m_mainShader.SetInt("_NumOfPointLights", m_pointLights.Count);
        ComputeBuffer pointLightBuf = null;
        if (m_pointLights.Count > 0)
        {
            pointLightBuf = new ComputeBuffer(m_pointLights.Count, RTLightStructurePoint_t.GetSize());
            pointLightBuf.SetData(m_pointLights);
        }
        else
        {
            pointLightBuf = new ComputeBuffer(1, 4); // Dummy
        }

        m_mainShader.SetBuffer(kIndex, "_PointLights", pointLightBuf);


        // Spot Lights

        m_mainShader.SetInt("_NumOfSpotLights", m_spotLights.Count);
        ComputeBuffer spotLightBuf = null;
        if (m_spotLights.Count > 0)
        {
            spotLightBuf = new ComputeBuffer(m_spotLights.Count, RTLightStructureSpot_t.GetSize());
            spotLightBuf.SetData(m_spotLights);
        }
        else
        {
            spotLightBuf = new ComputeBuffer(1, 4); // Dummy
        }

        m_mainShader.SetBuffer(kIndex, "_SpotLights", spotLightBuf);



        m_mainShader.SetTexture(kIndex, "Result", m_target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        if (threadGroupsX > 0 && threadGroupsY > 0
        ) // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        {
            m_mainShader.Dispatch(kIndex, threadGroupsX, threadGroupsY, 1);
        }


        sphereBuffer.Release();
        triangleBuffer.Release();
        dirLightBuf.Release();
        pointLightBuf.Release();
        spotLightBuf.Release();


        m_buffer.Blit(m_target, camera.activeTexture);

        renderContext
            .ExecuteCommandBuffer(
                m_buffer); // We copied all the commands to an internal memory that is ready to send to GPU
        m_buffer.Clear(); // Clear the command buffer

        #endregion

        // End Unity profiler sample for frame debugger
        m_buffer.EndSample(s_bufferName);
        renderContext
            .ExecuteCommandBuffer(
                m_buffer); // We copied all the commands to an internal memory that is ready to send to GPU
        m_buffer.Clear(); // Clear the command buffer


        renderContext.Submit(); // Send all the batched commands to GPU
    }


    


    /// <summary>
    /// This method ensure our RenderTexture is initialized properly for ComputeShader
    /// The code snipet is copied from http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
    /// Reference: http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
    /// </summary>
    private void InitRenderTexture()
    {
        if (m_target == null || m_target.width != Screen.width || m_target.height != Screen.height)
        {
            // Release render texture if we already have one
            if (m_target != null)
                m_target.Release();

            // Get a render target for Ray Tracing
            m_target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_target.enableRandomWrite = true;
            m_target.Create();
        }
    }
}