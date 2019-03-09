using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RayTracingRenderer
{
    public partial class RayTracingRenderPipeline
    {
        private void InitSceneParsing()
        {
            m_sceneParser = new SceneParser();
        }

        private void RunParseScene()
        {
            var scene = SceneManager.GetActiveScene();
            
            m_sceneParser.ParseScene(scene);
        }
    }
}

