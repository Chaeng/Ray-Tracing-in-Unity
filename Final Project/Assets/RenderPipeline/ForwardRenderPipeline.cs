using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;                // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering;   // Since SRP is an experimental feature, we have to import it using this namespace

public class ForwardRenderPipeline : RenderPipeline
{
    /// <summary>
    /// This method get called by Unity every frame to draw on the screen. 
    /// </summary>
    /// <param name="renderContext">Render context.</param>
    /// <param name="cameras">All running cameras</param>
    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        base.Render(renderContext, cameras);

        foreach(var camera in cameras)
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
        renderContext.SetupCameraProperties(camera);    // This tells the renderer the camera position and orientation, projection type (perspective/orthographic)


        #region Culling - Determine what objects should be rendered

        ScriptableCullingParameters cullingParameters;
        if(!CullResults.GetCullingParameters(camera, out cullingParameters))    // Unity helps us to fill in the parameters to perform the culling operations
        {
            return;     // However, if Unity cannot figure out how to do culling on this camera, then there is nothing to render. Return.
        }
        CullResults cull = CullResults.Cull(ref cullingParameters, renderContext);    // The actual cull operation

        #endregion



        #region Clear Flag - Clear the previous frame

        var buffer = new CommandBuffer { name = camera.name };  // We batch the commands into a buffer to reduce the amount of sending commands to GPU

        CameraClearFlags clearFlags = camera.clearFlags;        // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
        buffer.ClearRenderTarget(
            ((clearFlags & CameraClearFlags.Depth) != 0), 
            ((clearFlags & CameraClearFlags.Color) != 0), 
            camera.backgroundColor);     

        renderContext.ExecuteCommandBuffer(buffer);     // We copied all the commands to an internal memory that is ready to send to GPU
        buffer.Release();                               // Release the memory allocated by the buffer as all the commands already copied to internal memory

        #endregion



        #region Drawing - Opaque objects first (such that things befind itself does not need to be drawn)

        var drawSettings = new DrawRendererSettings(camera, new ShaderPassName("SRPDefaultUnlit"));     // This tells Unity to draw GameObjects with Unlit materials; SRPDefaultUnlit is Unity defined name of this operation

        drawSettings.sorting.flags = SortFlags.CommonOpaque;  // This tells Unity to sort all the objects in the scene, from closest to farest. Things that close to camera draw first to save drawing thing behind it

        var filterSettings = new FilterRenderersSettings(true)
        {
            // true == This tells Unity to include every GameObjects in the cull results

            renderQueueRange = RenderQueueRange.opaque      // This tells Unity only involves Opaque shader on each GameObject
        };
                                                 
        renderContext.DrawRenderers(cull.visibleRenderers, ref drawSettings, filterSettings);       // Draw

        #endregion



        #region Skybox - Everything the camera cannot seen will use the skybox colors

        renderContext.DrawSkybox(camera);   // Draw skybox

        #endregion



        #region Drawing - Transparent object

        filterSettings.renderQueueRange = RenderQueueRange.transparent;     // This time, we tells Unity only involves transparent shader on each GameObject

        drawSettings.sorting.flags = SortFlags.CommonTransparent;           // This tells Unity to sort all the objects in the scene, from farest to closest. This allows the transparent objects has every colors behind it

        renderContext.DrawRenderers(cull.visibleRenderers, ref drawSettings, filterSettings);       // Draw

        #endregion




        renderContext.Submit();     // Send all the batched commands to GPU
    }
}
