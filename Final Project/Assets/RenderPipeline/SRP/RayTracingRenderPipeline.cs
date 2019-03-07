using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering; // Since SRP is an experimental feature, we have to import it using this namespace
using UnityEngine.SceneManagement;


public partial class RayTracingRenderPipeline : RenderPipeline
{
    private readonly static string s_bufferName = "Ray Tracing Render Camera";

    private ComputeShader m_mainShader; // The compute shader we are going to write our ray tracing program on
    private ComputeShader m_shadowMapShader;

    private RenderTexture m_target; // The texture to hold the ray tracing result from the compute shader

    private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings      

    private RenderPipelineConfigObject m_config;

    private List<RTLightStructureDirectional_t> m_directionalLights;
    private List<RTLightStructurePoint_t> m_pointLights;
    private List<RTLightStructureSpot_t> m_spotLights;

    private List<RTSphere_t> m_sphereGeom;
    private List<RTTriangle_t> m_triangleGeom;

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


    private void ParseScene(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        ParseLight(roots);
        ParseSphere(roots);
        ParseTriangle(roots);
    }


    private void ParseSphere(GameObject[] roots)
    {
        // TODO: Optimize dynamic array generation
        if (m_sphereGeom == null)
        {
            m_sphereGeom = new List<RTSphere_t>();
        }

        m_sphereGeom.Clear();

        foreach (var root in roots)
        {
            RTSphereRenderer[] sphereRenderers = root.GetComponentsInChildren<RTSphereRenderer>();

            foreach (var renderer in sphereRenderers)
            {
                if (renderer.gameObject.activeSelf)
                {
                    m_sphereGeom.Add(renderer.GetGeometry());
                }
            }
        }
    }

    private void ParseTriangle(GameObject[] roots)
    {
        // TODO: Optimize dynamic array generation
        if (m_triangleGeom == null)
        {
            m_triangleGeom = new List<RTTriangle_t>();
        }

        m_triangleGeom.Clear();

        foreach (var root in roots)
        {
            RTTriangleRenderer[] triangleRenderers = root.GetComponentsInChildren<RTTriangleRenderer>();

            foreach (var renderer in triangleRenderers)
            {
                if (renderer.gameObject.activeSelf)
                {
                    m_triangleGeom.Add(renderer.GetGeometry());
                }
            }
        }
    }


    private void ParseLight(GameObject[] roots)
    {
        if (m_directionalLights == null)
        {
            m_directionalLights = new List<RTLightStructureDirectional_t>();
        }

        m_directionalLights.Clear();

        if (m_pointLights == null)
        {
            m_pointLights = new List<RTLightStructurePoint_t>();
        }

        m_pointLights.Clear();

        if (m_spotLights == null)
        {
            m_spotLights = new List<RTLightStructureSpot_t>();
        }

        m_spotLights.Clear();

        foreach (var root in roots)
        {
            Light[] lights = root.GetComponentsInChildren<Light>();

            foreach (var light in lights)
            {
                if (!light.gameObject.activeSelf)
                {
                    continue;
                }

                switch (light.type)
                {
                    case LightType.Directional:
                        {
                            Color lightColor = light.color;

                            RTLightStructureDirectional_t directional = new RTLightStructureDirectional_t();
                            directional.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            directional.direction = -1 * Vector3.Normalize(light.transform.rotation * Vector3.forward);
                            m_directionalLights.Add(directional);
                        }
                        break;

                    case LightType.Point:
                        {
                            Color lightColor = light.color;

                            RTLightStructurePoint_t point = new RTLightStructurePoint_t();
                            point.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            point.position = light.transform.position;
                            m_pointLights.Add(point);
                        }
                        break;

                    case LightType.Spot:
                        {
                            Color lightColor = light.color;

                            RTLightStructureSpot_t spot = new RTLightStructureSpot_t();
                            spot.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            spot.position = light.transform.position;
                            spot.direction = -1 * Vector3.Normalize(light.transform.rotation * Vector3.forward);
                            spot.coneAngle = light.spotAngle;
                            m_spotLights.Add(spot);
                        }
                        break;
                }
            }
        }
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



        #region Shadow Map Pass

        ShadowMapPass(Vector3.zero, Vector3.zero, m_sphereGeom.Count, sphereBuffer, m_triangleGeom.Count, triangleBuffer);

        #endregion



        #region Ray Tracing

        m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        m_mainShader.SetTexture(0, "_SkyboxTexture", m_config.skybox);

        // Sphere

        m_mainShader.SetInt("_NumOfSpheres", m_sphereGeom.Count);
        m_mainShader.SetBuffer(0, "_Spheres", sphereBuffer);

        // Triangle

        m_mainShader.SetInt("_NumOfTriangles", m_triangleGeom.Count);


        m_mainShader.SetBuffer(0, "_Triangles", triangleBuffer);

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

        m_mainShader.SetBuffer(0, "_DirectionalLights", dirLightBuf);

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

        m_mainShader.SetBuffer(0, "_PointLights", pointLightBuf);


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

        m_mainShader.SetBuffer(0, "_SpotLights", spotLightBuf);



        m_mainShader.SetTexture(0, "Result", m_target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        if (threadGroupsX > 0 && threadGroupsY > 0
        ) // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        {
            m_mainShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }


        sphereBuffer.Release();
        triangleBuffer.Release();
        dirLightBuf.Release();
        pointLightBuf.Release();


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


    private void LoadBufferWithSpheres(ref ComputeBuffer sphereBuffer)
    {
        if (m_sphereGeom.Count > 0)
        {
            sphereBuffer = new ComputeBuffer(m_sphereGeom.Count, 4 * sizeof(float));
            sphereBuffer.SetData(m_sphereGeom);
        }
        else
        {
            sphereBuffer = new ComputeBuffer(1, 16); // need to be at least 16 bytes long for RTSphere_t
        }
    }


    private ComputeBuffer LoadBufferWithTriangles()
    {
        ComputeBuffer triangleBuffer = null;
        if (m_triangleGeom.Count > 0)
        {
            triangleBuffer = new ComputeBuffer(m_triangleGeom.Count, RTTriangle_t.GetSize());
            triangleBuffer.SetData(m_triangleGeom);
        }
        else
        {
            triangleBuffer = new ComputeBuffer(1, RTTriangle_t.GetSize());
        }
        return triangleBuffer;
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