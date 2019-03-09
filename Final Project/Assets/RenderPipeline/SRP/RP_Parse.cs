using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Import Unity stuff related to rendering that is shared by all rendering pipelines
using UnityEngine.Experimental.Rendering; // Since SRP is an experimental feature, we have to import it using this namespace
using UnityEngine.SceneManagement;

public partial class RayTracingRenderPipeline : RenderPipeline
{
    private void ParseScene(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        ParseLight(roots);
        ParseSphere(roots);
        ParseTriangle(roots);
    }


    private void ParseSphere(GameObject[] roots)
    {
        // TODO: Optimize dynamic array generation
        if (m_sphereGeom == null)
        {
            m_sphereGeom = new List<RTSphere_t>();
        }

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
        if (m_directionalLights == null)
        {
            m_directionalLights = new List<RTLightStructureDirectional_t>();
        }

        m_directionalLights.Clear();

        if (m_pointLights == null)
        {
            m_pointLights = new List<RTLightStructurePoint_t>();
        }

        m_pointLights.Clear();

        if (m_spotLights == null)
        {
            m_spotLights = new List<RTLightStructureSpot_t>();
        }

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
                            spot.coneAngle = light.spotAngle * (Mathf.PI / 180);
                            m_spotLights.Add(spot);
                        }
                        break;
                }
            }
        }
    }
}
