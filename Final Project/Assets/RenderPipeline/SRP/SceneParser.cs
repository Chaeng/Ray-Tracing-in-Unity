//#define DEBUG_VERBOSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RayTracingRenderer
{
    public class SceneParser
    {
        private List<RTSphere_t> m_sphereGeom;
        private List<RTTriangle_t> m_triangleGeom;
        private List<RTMaterial_t> m_materials;
        private List<RTTexture_t> m_textures;
        private List<Texture> m_textureImages;
        private List<RTLightStructureDirectional_t> m_directionalLights;
        private List<RTLightStructurePoint_t> m_pointLights;
        private List<RTLightStructureSpot_t> m_spotLights;
        
        public SceneParser()
        {
            m_sphereGeom = new List<RTSphere_t>();
            m_triangleGeom = new List<RTTriangle_t>();

            m_directionalLights = new List<RTLightStructureDirectional_t>();
            m_pointLights = new List<RTLightStructurePoint_t>();
            m_spotLights = new List<RTLightStructureSpot_t>();
        }


        public List<RTSphere_t> GetSpheres()
        {
            return m_sphereGeom;
        }
        
        public List<RTTriangle_t> GetTriangles()
        {
            return m_triangleGeom;
        }
        
        public List<RTMaterial_t> GetMaterials()
        {
            return m_materials;
        }
        
        public List<RTTexture_t> GetTextures()
        {
            return m_textures;
        }

        public List<Texture> GetTextureImages()
        {
            return m_textureImages;
        }
        
        public List<RTLightStructureDirectional_t> GetDirectionalLights()
        {
            return m_directionalLights;
        }
        
        public List<RTLightStructurePoint_t> GetPointLights()
        {
            return m_pointLights;
        }
        
        public List<RTLightStructureSpot_t> GetSpotLights()
        {
            return m_spotLights;
        }
        
        
        public void ParseScene(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            int count = 0;
            int imageCount = 0;

            ParseLight(roots);
            ParseTexture(roots, ref imageCount);
            ParseMaterial(roots);
            ParseSphere(roots, ref count);
            ParseTriangle(roots,ref count);
        }

        private void ParseMaterial(GameObject[] roots)
        {
            int count = 0;

            // TODO: Optimize dynamic array generation
            if (m_materials == null)
            {
                m_materials = new List<RTMaterial_t>();
            }

            m_materials.Clear();

            foreach (var root in roots)
            {
                RTMaterialDatabase[] materialDatabases 
                    = root.GetComponentsInChildren<RTMaterialDatabase>();

                foreach (var database in materialDatabases)
                {
                    if (database.gameObject.activeSelf)
                    {
                        foreach (var mat in database.GetMaterials())
                        {
                            // Search the scene for geometries using this texture
                            // and update their indices to point back here
                            string name = mat.GetName();
                            SetMaterialIndexSpheres(roots, name, count);
                            SetMaterialIndexTriangles(roots, name, count);

                            // Add this material to static list for output
                            RTMaterial_t matStructure = mat.GetMaterial();
                            matStructure.id = count++;
                            m_materials.Add(matStructure);

#if DEBUG_VERBOSE
                            Debug.Log(string.Format("Add Mat {0} w/ "
                                + "\n textureIndexKa {1}"
                                + "\n textureIndexKd {2}"
                                + "\n textureIndexKs {3}"
                                + "\n textureIndexR {4}"
                                + "\n textureIndexT {5}",
                                mat.GetName(),
                                matStructure.textureIndexKa,
                                matStructure.textureIndexKd,
                                matStructure.textureIndexKs,
                                matStructure.textureIndexR,
                                matStructure.textureIndexT));
#endif
                        }
                    }
                }
            }
        }

        private void SetMaterialIndexSpheres(GameObject[] roots,
            string name, int index)
        {
            foreach (var root in roots)
            {
                RTSphereRenderer[] sphereRenderers 
                    = root.GetComponentsInChildren<RTSphereRenderer>();

                foreach (var renderer in sphereRenderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        if (renderer.GetSphere().GetMaterialName() == name)
                        {
                            renderer.GetSphere().SetMaterialIndex(index);
                        }
                    }
                }
            }
        }

        private void SetMaterialIndexTriangles(GameObject[] roots,
            string name, int index)
        {
            foreach (var root in roots)
            {
                RTTriangleRenderer[] triangleRenderers 
                    = root.GetComponentsInChildren<RTTriangleRenderer>();

                foreach (var renderer in triangleRenderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        if (renderer.GetTriangle().GetMaterialName() == name)
                        {
                            renderer.GetTriangle().SetMaterialIndex(index);
                        }
                    }
                }
            }
        }

        private void ParseTexture(GameObject[] roots, ref int imageCount)
        {
            int count = 0;

             // TODO: Optimize dynamic array generation
            if (m_textures == null)
            {
                m_textures = new List<RTTexture_t>();
            }

             // TODO: Optimize dynamic array generation
            if (m_textureImages == null)
            {
                m_textureImages = new List<Texture>();
            }

            m_textures.Clear();

            foreach (var root in roots)
            {
                RTTextureDatabase[] textureDatabases 
                    = root.GetComponentsInChildren<RTTextureDatabase>();

                foreach (var database in textureDatabases)
                {
                    if (database.gameObject.activeSelf)
                    {
                         foreach (var tex in database.GetTextures())
                        {
                            // Search the scene for materials using this texture
                            // and update their indices to point back here
                            string name = tex.GetName();
                            SetTextureIndexMaterials(roots, name, count);

                            // Add this texture to static list for output
                            RTTexture_t texStructure = tex.GetTexture();
                            if (tex.GetTexture().isColor != 0)
                            {
                                Texture texImage = tex.GetImage();
                                texStructure.imageIndex = imageCount++;
                                m_textureImages.Add(texImage);
                            }
                            texStructure.id = count++;  // arbitrary, unique
                            m_textures.Add(texStructure);

#if DEBUG_VERBOSE
                            Debug.Log(string.Format("Add Tex {0} w/ "
                                + "\n isColor {1}"
                                + "\n imageIndex {2}"
                                + "\n isChecker {3}"
                                + "\n uRepeat {4}"
                                + "\n vRepeat {5}"
                                + "\n color1 {6}"
                                + "\n color2 {7}",
                                tex.GetName(),
                                texStructure.isColor,
                                texStructure.imageIndex,
                                texStructure.isChecker,
                                texStructure.uRepeat,
                                texStructure.vRepeat,
                                texStructure.color1,
                                texStructure.color2));
#endif
                        }
                    }
                }
            }
        }

        private void SetTextureIndexMaterials(GameObject[] roots,
            string name,
            int index)
        {
            foreach (var root in roots)
            {
                RTMaterialDatabase[] materialDatabases 
                    = root.GetComponentsInChildren<RTMaterialDatabase>();

                foreach (var database in materialDatabases)
                {
                    if (database.gameObject.activeSelf)
                    {
                        foreach (var mat in database.GetMaterials())
                        {
                            if (mat.GetTextureNameKa() == name)
                            {
                                mat.SetTextureIndexKa(index);
                            }
                            if (mat.GetTextureNameKd() == name)
                            {
                                mat.SetTextureIndexKd(index);
                            }
                            if (mat.GetTextureNameKs() == name)
                            {
                                mat.SetTextureIndexKs(index);
                            }
                            // if (mat.GetTextureNameR() == name)
                            // {
                            //     mat.SetTextureIndexR(index);
                            // }
                            // if (mat.GetTextureNameT() == name)
                            // {
                            //     mat.SetTextureIndexT(index);
                            // }
                        }
                    }
                }
            }
        }

        private void ParseSphere(GameObject[] roots, ref int counter)
        {   
            // TODO: Optimize dynamic array generation
            m_sphereGeom.Clear();

            foreach (var root in roots)
            {
                RTSphereRenderer[] sphereRenderers = root.GetComponentsInChildren<RTSphereRenderer>();

                foreach (var renderer in sphereRenderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        RTSphere_t sphereGeomT = renderer.GetGeometry();
                        sphereGeomT.id = counter++;
                        m_sphereGeom.Add(sphereGeomT);
                    }
                }
            }
        }

        private void ParseTriangle(GameObject[] roots, ref int counter)
        {
            // TODO: Optimize dynamic array generation
            if (m_triangleGeom == null)
            {
                m_triangleGeom = new List<RTTriangle_t>();
            }

            m_triangleGeom.Clear();

            foreach (var root in roots)
            {
                RTTriangleRenderer[] triangleRenderers = root.GetComponentsInChildren<RTTriangleRenderer>();

                foreach (var renderer in triangleRenderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        RTTriangle_t triangleGeomT = renderer.GetGeometry();
                        triangleGeomT.id = counter++;
                        m_triangleGeom.Add(triangleGeomT);
                    }
                }
            }
        }

        private void ParseLight(GameObject[] roots)
        {
            m_directionalLights.Clear();

            m_pointLights.Clear();

            m_spotLights.Clear();

            foreach (var root in roots)
            {
                RTLight[] lights = root.GetComponentsInChildren<RTLight>();

                foreach (var light in lights)
                {
                    if (!light.gameObject.activeSelf)
                    {
                        continue;
                    }

                    switch (light.GetLightType())
                    {
                        case RTLight.LightType.Directional:
                        {
                            RTLightStructureDirectional_t directional = light.GetDirectionalLight();
                            m_directionalLights.Add(directional);
                        }
                            break;

                        case RTLight.LightType.Point:
                        {
                            RTLightStructurePoint_t point = light.GetPointLight();
                            m_pointLights.Add(point);
                        }
                            break;

                        case RTLight.LightType.Spot:
                        {
                            RTLightStructureSpot_t spot = light.GetSpotLight();
                            m_spotLights.Add(spot);
                        }
                            break;
                    }
                }
            }
        }
    }
}