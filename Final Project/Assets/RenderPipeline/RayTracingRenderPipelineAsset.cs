/*
 * =============================================
 * RayTracingRenderPipelineAsset
 * 
 * This is a script to :
 * 
 * (1) Generate an assets which represents the Ray Tracing Render Pipeline;
 * (2) Provide configuration UI to the render pipeline;
 * 
 * 
 * =============================================
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

// Since SRP is an experimental feature, we have to import it using this namespace


[CreateAssetMenu(menuName = "Rendering/RayTracingRenderPipeline")]
public class RayTracingRenderPipelineAsset : RenderPipelineAsset
{
    public RenderPipelineConfigObject m_defaultConfig;
    public List<RenderPipelineConfigObject> m_config;

    public ComputeShader computeShader;

    /// <summary>
    /// This return a render pipeline to Unity. It is used by Unity.
    /// </summary>
    /// <returns>The create pipeline.</returns>
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new RayTracingRenderPipeline(computeShader, m_config);
    }
}