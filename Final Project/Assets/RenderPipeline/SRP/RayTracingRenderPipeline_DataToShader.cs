using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayTracingRenderer
{
    public partial class RayTracingRenderPipeline
    {
        private void RunLoadGeometryToBuffer(ref ComputeBuffer sphereBuffer, ref ComputeBuffer triangleBuffer, SceneParser sceneParser)
        {
            LoadBufferWithSpheres(ref sphereBuffer, sceneParser);
            LoadBufferWithTriangles(ref triangleBuffer, sceneParser);
        }


        private void RunLoadLightsToBuffer(ref ComputeBuffer directionalBuffer, ref ComputeBuffer pointBuffer,
            ref ComputeBuffer spotBuffer, SceneParser sceneParser)
        {
            LoadBufferWithDirectionalLights(ref directionalBuffer, sceneParser);
            LoadBufferWithPointLights(ref pointBuffer, sceneParser);
            LoadBufferWithSpotLights(ref spotBuffer, sceneParser);
        }
        
        
        private void LoadBufferWithSpheres(ref ComputeBuffer sphereBuffer, SceneParser sceneParser)
        {
            int sphereCount = sceneParser.GetSpheres().Count;
            
            if (sphereCount > 0)
            {
                sphereBuffer = new ComputeBuffer(sphereCount, sizeof(float)*4);
                sphereBuffer.SetData(sceneParser.GetSpheres());
            }
            else
            {
                sphereBuffer = new ComputeBuffer(1, sizeof(float) * 4);
            }
        }


        private void LoadBufferWithTriangles(ref ComputeBuffer triangleBuffer, SceneParser sceneParser)
        {
            int triCount = sceneParser.GetTriangles().Count;
            
            if (triCount > 0)
            {
                triangleBuffer = new ComputeBuffer(triCount, RTTriangle_t.GetSize());
                triangleBuffer.SetData(sceneParser.GetTriangles());
            }
            else
            {
                triangleBuffer = new ComputeBuffer(1, RTTriangle_t.GetSize());
            }
        }


        private void LoadBufferWithDirectionalLights(ref ComputeBuffer buffer, SceneParser sceneParser)
        {
            int count = sceneParser.GetDirectionalLights().Count;
            
            if (count > 0)
            {
                buffer = new ComputeBuffer(count, RTLightStructureDirectional_t.GetSize());
                buffer.SetData(sceneParser.GetDirectionalLights());
            }
            else
            {
                buffer = new ComputeBuffer(1, 4); // Dummy
            }
        }
        
        private void LoadBufferWithPointLights(ref ComputeBuffer buffer, SceneParser sceneParser)
        {
            int count = sceneParser.GetPointLights().Count;
            
            if (count > 0)
            {
                buffer = new ComputeBuffer(count, RTLightStructurePoint_t.GetSize());
                buffer.SetData(sceneParser.GetPointLights());
            }
            else
            {
                buffer = new ComputeBuffer(1, 4); // Dummy
            }
        }
        
        private void LoadBufferWithSpotLights(ref ComputeBuffer buffer, SceneParser sceneParser)
        {
            int count = sceneParser.GetSpotLights().Count;
            
            if (count > 0)
            {
                buffer = new ComputeBuffer(count, RTLightStructureSpot_t.GetSize());
                buffer.SetData(sceneParser.GetSpotLights());
            }
            else
            {
                buffer = new ComputeBuffer(1, 4); // Dummy
            }
        }

        private void RunBufferCleanUp()
        {
            m_sphereBuffer.Release();
            m_triangleBuffer.Release();
            m_directionalLightBuffer.Release();
            m_pointLightBuffer.Release();
            m_spotLightBuffer.Release();
            m_shadowUtilityBuffer.Release();
        }
    }
}
