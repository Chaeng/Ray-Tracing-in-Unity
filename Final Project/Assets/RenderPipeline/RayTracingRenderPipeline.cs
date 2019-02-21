using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;                // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering;   // Since SRP is an experimental feature, we have to import it using this namespace

public class RayTracingRenderPipeline : RenderPipeline
{
    private ComputeShader m_computeShader;  // The compute shader we are going to write our ray tracing program on

    private RenderTexture m_target;         // The texture to hold the ray tracing result from the compute shader
    private Texture m_skyboxTex;            // The skybox we used as the background


    public RayTracingRenderPipeline(ComputeShader computeShader, Texture skybox)
    {
        m_computeShader = computeShader;
        m_skyboxTex = skybox;
    }



    /// <summary>
    /// This method get called by Unity every frame to draw on the screen. 
    /// </summary>
    /// <param name="renderContext">Render context.</param>
    /// <param name="cameras">All running cameras</param>
    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        base.Render(renderContext, cameras);

        foreach (var camera in cameras)
        {
            RenderPerCamera(renderContext, camera);
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

        renderContext.SetupCameraProperties(camera);    // This tells the renderer the camera position and orientation, projection type (perspective/orthographic)


        #region Clear Flag - Clear the previous frame

        var buffer = new CommandBuffer { name = "Ray Tracing Renderer" };  // We batch the commands into a buffer to reduce the amount of sending commands to GPU

        CameraClearFlags clearFlags = camera.clearFlags;        // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
        buffer.ClearRenderTarget(
            ((clearFlags & CameraClearFlags.Depth) != 0),
            ((clearFlags & CameraClearFlags.Color) != 0),
            camera.backgroundColor);


        m_computeShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
        m_computeShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        m_computeShader.SetTexture(0, "_SkyboxTexture", m_skyboxTex);
        m_computeShader.SetTexture(0, "Result", m_target);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        if (threadGroupsX > 0 && threadGroupsY > 0)                 // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
        {
            m_computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }

        buffer.Blit(m_target, camera.activeTexture);

        renderContext.ExecuteCommandBuffer(buffer);     // We copied all the commands to an internal memory that is ready to send to GPU
        buffer.Release();                               // Release the memory allocated by the buffer as all the commands already copied to internal memory

        #endregion





        renderContext.Submit();     // Send all the batched commands to GPU
    }



    /// <summary>
    /// This method ensure our RenderTexture is initialized properly for ComputeShader
    /// The code snipet is copied from http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
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
