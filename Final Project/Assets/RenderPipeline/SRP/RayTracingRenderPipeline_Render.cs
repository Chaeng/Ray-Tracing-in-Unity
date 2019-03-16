using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace RayTracingRenderer
{
    public partial class RayTracingRenderPipeline
    {
        private readonly static string s_bufferName = "Ray Tracing Render Camera";

        private int kIndex = 0;


        private void InitRender(ref CommandBuffer buffer, ComputeShader mainShader)
        {
            // We batch the commands into a buffer to reduce the amount of sending commands to GPU
            // Reusing the command buffer object avoids continuous memory allocation

            buffer = new CommandBuffer
            {
                name = s_bufferName
            };

            kIndex = mainShader.FindKernel("CSMain");
        }


        /// <summary>
        /// This method ensure our RenderTexture is initialized properly for ComputeShader
        /// The code snipet is copied from http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
        /// Reference: http://blog.three-eyed-games.com/2018/05/03/gpu-ray-tracing-in-unity-part-1/
        /// </summary>
        private void RunTargetTextureInit(ref RenderTexture targetTexture)
        {
            if (targetTexture == null || targetTexture.width != Screen.width || targetTexture.height != Screen.height)
            {
                // Release render texture if we already have one
                if (targetTexture != null)
                {
                    targetTexture.Release();
                }

                // Get a render target for Ray Tracing
                targetTexture = new RenderTexture(Screen.width, Screen.height, 0,
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                targetTexture.enableRandomWrite = true;
                targetTexture.Create();
            }
        }


        private void RunClearCanvas(Camera camera)
        {
            CameraClearFlags
                clearFlags =
                    camera.clearFlags; // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
            m_buffer.ClearRenderTarget(
                ((clearFlags & CameraClearFlags.Depth) != 0),
                ((clearFlags & CameraClearFlags.Color) != 0),
                camera.backgroundColor);
        }


        private void RunCommandBufferBegin()
        {
            // Begin Unity profiler sample for frame debugger
            m_buffer.BeginSample(s_bufferName);
        }


        private void RunSetCameraToMainShader(Camera camera)
        {
            m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
            m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        }

        private void RunSetShadowMapToMainShader(Texture2DArray shadowMapList, ComputeBuffer shadowUtilityBuffer)
        {
            // Shadow Depth Map for Spot Light
            m_mainShader.SetTexture(kIndex, "_SpotShadowMap", shadowMapList);
            m_mainShader.SetBuffer(kIndex, "_ShadowUtility", shadowUtilityBuffer);
        }

        private void RunSetSkyboxToMainShader(Texture skybox)
        {
            m_mainShader.SetTexture(kIndex, "_SkyboxTexture", skybox);
        }

        private void RunSetAmbientToMainShader(RenderPipelineConfigObject config)
        {
            m_mainShader.SetVector("_AmbientLightUpper", config.upperAmbitent);
            m_mainShader.SetVector("_AmbientLightLower", config.lowerAmbitent);
        }

        private void RunSetGlobalRefractiveIndex(RenderPipelineConfigObject config)
        {
            m_mainShader.SetFloat("_GlobalRefractiveIndex", config.globalRefractiveIndex);
        }

        private void RunSetRayGeneration(RenderPipelineConfigObject config)
        {
            m_mainShader.SetInt("_MaxRayGeneration", config.maxRayGeneration);
        }

        private void RunSetFogToMainShader(RenderPipelineConfigObject config)
        {
            m_mainShader.SetFloat("_FogFactor", config.fogFactor);
            m_mainShader.SetVector("_FogColor", config.fogColor);
        }

        private void RunSetSpheresToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfSpheres", count);
            m_mainShader.SetBuffer(kIndex, "_Spheres", buffer);
        }

        private void RunSetMaterialsToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfMaterials", count);
            m_mainShader.SetBuffer(kIndex, "_Materials", buffer);
        }

        private void RunSetTrianglesToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfTriangles", count);
            m_mainShader.SetBuffer(kIndex, "_Triangles", buffer);
        }

        private void RunSetDirectionalLightsToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfDirectionalLights", count);
            m_mainShader.SetBuffer(kIndex, "_DirectionalLights", buffer);
        }

        private void RunSetPointLightsToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfPointLights", count);
            m_mainShader.SetBuffer(kIndex, "_PointLights", buffer);
        }

        private void RunSetSpotLightsToMainShader(ComputeBuffer buffer, int count)
        {
            m_mainShader.SetInt("_NumOfSpotLights", count);
            m_mainShader.SetBuffer(kIndex, "_SpotLights", buffer);
        }

        private void RunRayTracing(RenderTexture targetTexture)
        {
            m_mainShader.SetTexture(kIndex, "Result", targetTexture);
            int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

            // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
            if (threadGroupsX > 0 && threadGroupsY > 0)
            {
                m_mainShader.Dispatch(kIndex, threadGroupsX, threadGroupsY, 1);
            }
        }

        private void RunSendTextureToUnity(CommandBuffer buffer, RenderTexture targeTexture,
            ScriptableRenderContext renderContext, Camera camera)
        {
            buffer.Blit(targeTexture, camera.activeTexture);

            renderContext
                .ExecuteCommandBuffer(
                    buffer); // We copied all the commands to an internal memory that is ready to send to GPU
            buffer.Clear(); // Clear the command buffer
            
            // End Unity profiler sample for frame debugger
            buffer.EndSample(s_bufferName);
            renderContext
                .ExecuteCommandBuffer(
                    buffer); // We copied all the commands to an internal memory that is ready to send to GPU
            buffer.Clear(); // Clear the command buffer


            renderContext.Submit(); // Send all the batched commands to GPU
        }
    }
}