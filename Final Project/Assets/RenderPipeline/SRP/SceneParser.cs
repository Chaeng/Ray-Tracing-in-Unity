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

            int counter = 0;

            ParseLight(roots);
            ParseSphere(roots, counter);
            ParseTriangle(roots, counter);
        }

        private void ParseSphere(GameObject[] roots, int counter)
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
                        sphereGeomT.id = counter;
                        counter++;
                        m_sphereGeom.Add(sphereGeomT);
                    }
                }
            }
        }

        private void ParseTriangle(GameObject[] roots, int counter)
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
                        triangleGeomT.id = counter;
                        counter++;
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