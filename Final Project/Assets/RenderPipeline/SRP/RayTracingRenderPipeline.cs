using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;


namespace RayTracingRenderer
{
    /// <summary>
    /// The rendering pipeline configured to use ray tracing as the rendering mechanism
    /// </summary>
    public partial class RayTracingRenderPipeline : RenderPipeline
    {
        /**
         * This is the main file for the RayTracingRenderPipeline.
         * Please only have these three things here:
         * (1) Member fields
         * (2) Constructors
         * (3) Main function
         *
         * Any fields declared below should be properly initialized in corresponding InitXXX() method
         */


        // Constructor parameters
        
        private ComputeShader m_mainShader; // The compute shader we are going to write our ray tracing program on
        private ComputeShader m_shadowMapShader;
        private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings   
        
        
        // Constants
        private const int ShadowMapSize = 64;    // TODO: Get this dimension from RenderConfig
        
        
        // Own member fields
        private CommandBuffer m_buffer;
        private ComputeBuffer m_directionalLightBuffer;
        private ComputeBuffer m_pointLightBuffer;
        private ComputeBuffer m_shadowUtilityBuffer;
        private ComputeBuffer m_sphereBuffer;
        private ComputeBuffer m_spotLightBuffer;
        private ComputeBuffer m_triangleBuffer;
        private RenderPipelineConfigObject m_config;
        private RenderTexture m_shadowMap;
        private RenderTexture m_target;
        private SceneParser m_sceneParser;
        private Texture2DArray m_shadowMapList;
        private List<ShadowUtility_t> m_shadowUtility;


        /// <summary>
        /// Construct the SRP with the following data
        /// </summary>
        /// <param name="mainShader">Compute shader containing ray tracing program</param>
        /// <param name="shadowMapShader">Compute shader containing shadow map program</param>
        /// <param name="allConfig">SRP configuration files</param>
        public RayTracingRenderPipeline(ComputeShader mainShader, ComputeShader shadowMapShader,
            List<RenderPipelineConfigObject> allConfig)
        {
            m_mainShader = mainShader;
            m_shadowMapShader = shadowMapShader;
            m_allConfig = allConfig;
            
            // Run all inits
            InitConfig(m_allConfig, ref m_config);
            InitShadowMap(ShadowMapSize);    // TODO: Get this dimension from RenderConfig
            InitRender(ref m_buffer, m_mainShader);
            InitSceneParsing();
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


            RunConfig(m_allConfig, ref m_config);
            RunParseScene();
            
            
            foreach (var camera in cameras)
            {
                RunTargetTextureInit(ref m_target);
                RunClearCanvas(camera);
                RunCommandBufferBegin();
                
                RunLoadGeometryToBuffer(m_sceneParser);
                RunLoadLightsToBuffer(ref m_directionalLightBuffer, ref m_pointLightBuffer, ref m_spotLightBuffer, m_sceneParser);
                
                RunShadowMap(ref m_shadowMapList, ref m_shadowUtilityBuffer, m_sceneParser, ShadowMapSize, m_sphereBuffer, m_triangleBuffer);

                RunSetCameraToMainShader(camera);
                RunSetSkyboxToMainShader(m_config.skybox);
                RunSetAmbientToMainShader(m_config.ambitent);
                RunSetShadowMapToMainShader(m_shadowMapList, m_shadowUtilityBuffer);
                RunSetSpheresToMainShader(m_sphereBuffer, m_sceneParser.GetSpheres().Count);
                RunSetTrianglesToMainShader(m_triangleBuffer, m_sceneParser.GetTriangles().Count);
                RunSetDirectionalLightsToMainShader(m_directionalLightBuffer, m_sceneParser.GetDirectionalLights().Count);
                RunSetPointLightsToMainShader(m_pointLightBuffer, m_sceneParser.GetPointLights().Count);
                RunSetSpotLightsToMainShader(m_spotLightBuffer, m_sceneParser.GetSpotLights().Count);
                RunRayTracing(m_target);
                RunBufferCleanUp();
                RunSendTextureToUnity(m_buffer, m_target, renderContext, camera);
            }
        }
    }
}