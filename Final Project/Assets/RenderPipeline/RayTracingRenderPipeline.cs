﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;                // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering;   // Since SRP is an experimental feature, we have to import it using this namespace
using UnityEngine.SceneManagement;


public class RayTracingRenderPipeline : RenderPipeline
{
    private readonly static string s_bufferName = "Ray Tracing Render Camera";

    private ComputeShader m_computeShader;  // The compute shader we are going to write our ray tracing program on

    private RenderTexture m_target;         // The texture to hold the ray tracing result from the compute shader
    private RenderPipelineConfigObject m_config;    // The config object containing all global rendering settings

    private List<RTLightStructureDirectional_t> m_directionalLights;
    
    private List<RTSphere_t> m_sphereGeom;

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
    public RayTracingRenderPipeline(ComputeShader computeShader, RenderPipelineConfigObject config)
    {
        m_computeShader = computeShader;

        m_config = config;
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

        ParseScene(SceneManager.GetActiveScene());

        foreach (var camera in cameras)
        {
            RenderPerCamera(renderContext, camera);
        }
    }



    private void ParseScene(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        ParseLight(roots);
        ParseSphere(roots);
    }


    private void ParseSphere(GameObject[] roots)
    {
        // TODO: Optimize dynamic array generation
        if(m_sphereGeom == null)
        {
            m_sphereGeom = new List<RTSphere_t>();
        }
        m_sphereGeom.Clear();

        foreach (var root in roots)
        {
            RTSphereRenderer[] sphereRenderers = root.GetComponentsInChildren<RTSphereRenderer>();

            foreach (var renderer in sphereRenderers)
            {
                m_sphereGeom.Add(renderer.GetGeometry());
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
        
        foreach (var root in roots)
        {
            Light[] lights = root.GetComponentsInChildren<Light>();

            foreach (var light in lights)
            {
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

                        break;
                    
                    case LightType.Spot:

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
        CameraClearFlags clearFlags = camera.clearFlags;        // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
        m_buffer.ClearRenderTarget(
            ((clearFlags & CameraClearFlags.Depth) != 0),
            ((clearFlags & CameraClearFlags.Color) != 0),
            camera.backgroundColor);
        #endregion
        
        // Begin Unity profiler sample for frame debugger
        m_buffer.BeginSample(s_bufferName);
        
        #region Ray Tracing

        m_computeShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        m_computeShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        m_computeShader.SetTexture(0, "_SkyboxTexture", m_config.skybox);
        m_computeShader.SetInt("_NumOfSpheres", m_sphereGeom.Count);
        ComputeBuffer sphereBuffer = null;
        if (m_sphereGeom.Count> 0)
        {
            sphereBuffer = new ComputeBuffer(m_sphereGeom.Count, 4*sizeof(float));
            sphereBuffer.SetData(m_sphereGeom);
        }
        else
        {
            sphereBuffer = new ComputeBuffer(1, 16);     // need to be at least 16 bytes long for RTSphere_t
        }
        m_computeShader.SetBuffer(0, "_Spheres", sphereBuffer);
        m_computeShader.SetVector("_AmbientGlobal", m_config.ambitent);
        
        // Directional Lights
        
        m_computeShader.SetInt("_NumOfDirectionalLights", m_directionalLights.Count);
        ComputeBuffer dirLightBuf = null;
        if (m_directionalLights.Count > 0)
        {
            dirLightBuf = new ComputeBuffer(m_directionalLights.Count, RTLightStructureDirectional_t.GetSize());
            dirLightBuf.SetData(m_directionalLights);
        }
        else
        {
            dirLightBuf = new ComputeBuffer(1, 4);    // Dummy
        }
        m_computeShader.SetBuffer(0, "_DirectionalLights", dirLightBuf);
        
        
        m_computeShader.SetTexture(0, "Result", m_target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        if (threadGroupsX > 0 && threadGroupsY > 0)                 // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        {
            m_computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }

        
        sphereBuffer.Release();
        dirLightBuf.Release();
        

        m_buffer.Blit(m_target, camera.activeTexture);

        renderContext.ExecuteCommandBuffer(m_buffer);     // We copied all the commands to an internal memory that is ready to send to GPU
        m_buffer.Clear();                                 // Clear the command buffer


        #endregion

        // End Unity profiler sample for frame debugger
        m_buffer.EndSample(s_bufferName);
        renderContext.ExecuteCommandBuffer(m_buffer);     // We copied all the commands to an internal memory that is ready to send to GPU
        m_buffer.Clear();                                 // Clear the command buffer




        renderContext.Submit();     // Send all the batched commands to GPU
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
