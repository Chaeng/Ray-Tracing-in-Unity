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

            ParseLight(roots);
            ParseSphere(roots);
            ParseTriangle(roots);
        }

        private void ParseSphere(GameObject[] roots)
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
                        m_sphereGeom.Add(renderer.GetGeometry());
                    }
                }
            }
        }

        private void ParseTriangle(GameObject[] roots)
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
                        m_triangleGeom.Add(renderer.GetGeometry());
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
                Light[] lights = root.GetComponentsInChildren<Light>();

                foreach (var light in lights)
                {
                    if (!light.gameObject.activeSelf)
                    {
                        continue;
                    }

                    switch (light.type)
                    {
                        case LightType.Directional:
                        {
                            Color lightColor = light.color;

                            RTLightStructureDirectional_t directional = new RTLightStructureDirectional_t();
                            directional.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            directional.direction = -1 * Vector3.Normalize(light.transform.forward);
                            m_directionalLights.Add(directional);
                        }
                            break;

                        case LightType.Point:
                        {
                            Color lightColor = light.color;

                            RTLightStructurePoint_t point = new RTLightStructurePoint_t();
                            point.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            point.position = light.transform.position;
                            m_pointLights.Add(point);
                        }
                            break;

                        case LightType.Spot:
                        {
                            Color lightColor = light.color;

                            RTLightStructureSpot_t spot = new RTLightStructureSpot_t();
                            spot.color = new Vector3(lightColor.r, lightColor.g, lightColor.b);
                            spot.position = light.transform.position;
                            spot.direction = -1 * Vector3.Normalize(light.transform.forward);
                            spot.coneAngle = light.spotAngle * Mathf.Deg2Rad;
                            spot.cosConeAngle = Mathf.Cos(spot.coneAngle/2f);
                            m_spotLights.Add(spot);
                        }
                            break;
                    }
                }
            }
        }
    }
}