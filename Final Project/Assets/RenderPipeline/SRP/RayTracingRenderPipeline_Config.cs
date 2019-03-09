using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RayTracingRenderer
{
    public partial class RayTracingRenderPipeline
    {
        private void InitConfig(List<RenderPipelineConfigObject> configs, ref RenderPipelineConfigObject myConfig)
        {
            myConfig = configs[0];
        }


        private void RunConfig(List<RenderPipelineConfigObject> configs, ref RenderPipelineConfigObject myConfig)
        {
            var scene = SceneManager.GetActiveScene();

            ApplyRenderConfig(scene, configs, ref myConfig);
        }
        
        
        private void ApplyRenderConfig(Scene scene, List<RenderPipelineConfigObject> configs, ref RenderPipelineConfigObject myConfig)
        {
            var sceneIndex = scene.buildIndex;

            if (configs.Count == 0)
            {
                return;
            }

            if (sceneIndex >= 0 && sceneIndex < configs.Count)
            {
                myConfig = configs[sceneIndex];
                return;
            }

            // No matching scene index, use the first one
            myConfig = configs[0];
        }
    }
}

