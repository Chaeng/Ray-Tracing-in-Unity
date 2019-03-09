using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayTracingRenderer
{
    public partial class RayTracingRenderPipeline
    {
        private void RunLoadGeometryToBuffer(SceneParser sceneParser)
        {
            LoadBufferWithSpheres(sceneParser);
            LoadBufferWithTriangles(sceneParser);
        }


        private void RunLoadLightsToBuffer(ref ComputeBuffer directionalBuffer, ref ComputeBuffer pointBuffer,
            ref ComputeBuffer spotBuffer, SceneParser sceneParser)
        {
            LoadBufferWithDirectionalLights(ref directionalBuffer, sceneParser);
            LoadBufferWithPointLights(ref pointBuffer, sceneParser);
            LoadBufferWithSpotLights(ref spotBuffer, sceneParser);
        }
        
        
        private void LoadBufferWithSpheres(SceneParser sceneParser)
        {
            //TODO: Can optimize memory usage by checking whether the number of sphere remains the same
            
            int sphereCount = sceneParser.GetSpheres().Count;

            m_sphereBuffer?.Release();

            if (sphereCount > 0)
            {
                m_sphereBuffer = new ComputeBuffer(sphereCount, sizeof(float)*4);
                m_sphereBuffer.SetData(sceneParser.GetSpheres());
            }
            else
            {
                m_sphereBuffer = new ComputeBuffer(1, sizeof(float) * 4);
            }
        }


        private void LoadBufferWithTriangles(SceneParser sceneParser)
        {
            int triCount = sceneParser.GetTriangles().Count;
            
            m_triangleBuffer?.Release();
            
            if (triCount > 0)
            {
                m_triangleBuffer = new ComputeBuffer(triCount, RTTriangle_t.GetSize());
                m_triangleBuffer.SetData(sceneParser.GetTriangles());
            }
            else
            {
                m_triangleBuffer = new ComputeBuffer(1, RTTriangle_t.GetSize());
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
