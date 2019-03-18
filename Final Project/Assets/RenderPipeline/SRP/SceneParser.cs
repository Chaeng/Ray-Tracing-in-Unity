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
        private List<RTLightStructureDirectional_t> m_directionalLights;
        private List<RTLightStructurePoint_t> m_pointLights;
        private List<RTLightStructureSpot_t> m_spotLights;

        private AccelGridGroup _accelGrids;

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

        public void GetAccelerateGridGeometryIndex(out List<RTTriangle_t> triangles, out List<int> gridIndex)
        {
            _accelGrids.GetTrianglesInGrids(out triangles, out gridIndex);
        }
        
        public List<RTMaterial_t> GetMaterials()
        {
            return m_materials;
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

            ParseLight(roots);
            ParseMaterial(roots);
            ParseSphere(roots, ref count);
            ParseTriangle(roots, ref count);
            ParseMesh(roots, ref count);
            
            ConstructAccelerateStructure();
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
                        if (database.GetMaterials() == null)
                        {
                            continue;
                        }
                        
                        foreach (var mat in database.GetMaterials())
                        {
                            string name = mat.GetName();
                            SetMaterialIndexSpheres(roots, name, count);
                            SetMaterialIndexTriangles(roots, name, count);

                            RTMaterial_t matStructure = mat.GetMaterial();
                            matStructure.id = count++;
                            m_materials.Add(matStructure);
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
                        string sphereMaterialName
                            = renderer.GetSphere().GetMaterialName();
                        if (sphereMaterialName != null)
                        {
                            if (sphereMaterialName == name)
                            {
                                renderer.GetSphere().SetMaterialIndex(index);
                            }
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
                        string triangleMaterialName
                            = renderer.GetTriangle().GetMaterialName();
                        if (triangleMaterialName != null)
                        {
                            if(triangleMaterialName == name)
                            {
                                renderer.GetTriangle().SetMaterialIndex(index);
                            }
                        }
                    }
                }
            }
        }

        private void ParseSphere(GameObject[] roots, ref int counter)
        {   
            // Force initialization of dependencies
            if (m_materials == null)
            {
                ParseMaterial(roots);
            }

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
            // Force initialization of dependencies
            if (m_materials == null)
            {
                ParseMaterial(roots);
            }

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


        private void ParseMesh(GameObject[] roots, ref int counter)
        {
            // TODO: Optimize dynamic array generation
            if (m_triangleGeom == null)
            {
                m_triangleGeom = new List<RTTriangle_t>();
            }

            foreach (var root in roots)
            {
                RTMeshRenderer[] meshRenderers = root.GetComponentsInChildren<RTMeshRenderer>();

                foreach (var renderer in meshRenderers)
                {
                    if (renderer.gameObject.activeSelf)
                    {
                        List<RTTriangle_t> allTrianglesInMesh = renderer.GetGeometry();

                        if(allTrianglesInMesh == null)
                        {
                            continue;
                        }

                        for(int t = 0; t < allTrianglesInMesh.Count; t++)
                        {
                            allTrianglesInMesh[t].SetId(counter);
                            counter++;
                            m_triangleGeom.Add(allTrianglesInMesh[t]);
                        }
                    }
                }
            }
        }


        private void ConstructAccelerateStructure()
        {
            bool isFirstTriangle = true;
            Vector3 localMin = Vector3.zero;
            Vector3 localMax = Vector3.zero;

            for (int i = 0; i < m_triangleGeom.Count; i++)
            {
                if (isFirstTriangle)
                {
                    isFirstTriangle = false;
                    localMin = m_triangleGeom[i].vert0;
                    localMax = m_triangleGeom[i].vert0;
                }
                        
                localMin.x = Mathf.Min(localMin.x, m_triangleGeom[i].vert0.x, m_triangleGeom[i].vert1.x, m_triangleGeom[i].vert2.x);
                localMin.y = Mathf.Min(localMin.y, m_triangleGeom[i].vert0.y, m_triangleGeom[i].vert1.y, m_triangleGeom[i].vert2.y);
                localMin.z = Mathf.Min(localMin.z, m_triangleGeom[i].vert0.z, m_triangleGeom[i].vert1.z, m_triangleGeom[i].vert2.z);
                            
                localMax.x = Mathf.Max(localMax.x, m_triangleGeom[i].vert0.x, m_triangleGeom[i].vert1.x, m_triangleGeom[i].vert2.x);
                localMax.y = Mathf.Max(localMax.y, m_triangleGeom[i].vert0.y, m_triangleGeom[i].vert1.y, m_triangleGeom[i].vert2.y);
                localMax.z = Mathf.Max(localMax.z, m_triangleGeom[i].vert0.z, m_triangleGeom[i].vert1.z, m_triangleGeom[i].vert2.z);
            }

            
            if (_accelGrids == null)
            {
                _accelGrids = new AccelGridGroup();
            }
            _accelGrids.UpdateAccelGridGroup(localMin, localMax);

            
            for (int i = 0; i < m_triangleGeom.Count; i++)
            {
                _accelGrids.AddTriangle(m_triangleGeom[i]);
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
