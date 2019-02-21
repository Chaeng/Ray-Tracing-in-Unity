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
using UnityEngine.Experimental.Rendering;   // Since SRP is an experimental feature, we have to import it using this namespace


[CreateAssetMenu(menuName = "Rendering/RayTracingPipeline")]
public class RayTracingRenderPipelineAsset : RenderPipelineAsset
{
    public ComputeShader computeShader;
    public Texture skybox;

    /// <summary>
    /// This return a render pipeline to Unity. It is used by Unity.
    /// </summary>
    /// <returns>The create pipeline.</returns>
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new RayTracingRenderPipeline(computeShader, skybox);
    }
}
