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
            LoadBufferWithMaterials(sceneParser);
            LoadBufferWithTextures(sceneParser);
            LoadBufferWithTextureImages(sceneParser);
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
                m_sphereBuffer = new ComputeBuffer(sphereCount, RTSphere_t.GetSize());
                m_sphereBuffer.SetData(sceneParser.GetSpheres());
            }
            else
            {
                m_sphereBuffer = new ComputeBuffer(1, RTSphere_t.GetSize());
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

        private void LoadBufferWithMaterials(SceneParser sceneParser)
        {
            int count = sceneParser.GetMaterials().Count;
            
            m_materialBuffer?.Release();
            
            if (count > 0)
            {
                m_materialBuffer = new ComputeBuffer(count, RTMaterial_t.GetSize());
                m_materialBuffer.SetData(sceneParser.GetMaterials());
            }
            else
            {
                m_materialBuffer = new ComputeBuffer(1, RTMaterial_t.GetSize());
            }
        }

        private void LoadBufferWithTextures(SceneParser sceneParser)
        {
            int count = sceneParser.GetTextures().Count;
            
            m_textureBuffer?.Release();
            
            if (count > 0)
            {
                m_textureBuffer = new ComputeBuffer(count, RTTexture_t.GetSize());
                m_textureBuffer.SetData(sceneParser.GetTextures());
            }
            else
            {
                m_textureBuffer = new ComputeBuffer(1, RTTexture_t.GetSize());
            }
        }

        private void LoadBufferWithTextureImages(SceneParser sceneParser)
        {
            // TODO: Support more image types, not just 64x64 RGBA32
            // by creating TextureImageUtility and passing it to compute shader

            List<Texture> textureImages = sceneParser.GetTextureImages();
            int count = textureImages == null
                ? 0
                : textureImages.Count;
            Debug.Log(string.Format("LoadBufferWithTextureImages: count={0}, textureImages null?={1}", count, textureImages == null));


            if (count > 0)
            {
                m_textureImageList = new Texture2DArray(TextureImageSize,
                    TextureImageSize, count, TextureFormat.RGBA32,
                    false, false);

                for (int i = 0; i < count; i++)
                {
                    Debug.Log(string.Format("LoadBufferWithTextureImages: Copying Tex id={0}, null?={1}", i, textureImages[i] == null));
                    Graphics.CopyTexture(textureImages[i], 0, 0, m_textureImageList, i, 0); // index is the index of the texture
                }
            }
            else
            {
                m_textureImageList = new Texture2DArray(TextureImageSize,
                    TextureImageSize, 1, TextureFormat.RGBAFloat,
                    false, false);
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
            m_materialBuffer.Release();
            m_textureBuffer.Release();
            m_directionalLightBuffer.Release();
            m_pointLightBuffer.Release();
            m_spotLightBuffer.Release();
            m_shadowUtilityBuffer.Release();
        }
    }
}
