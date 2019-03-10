using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTLight : MonoBehaviour
{
    [SerializeField] private LightType m_lightType = LightType.Directional;
    [SerializeField] public Color color = Color.white;

    [SerializeField, HideInInspector]
    public float coneAngle = 45;

    [SerializeField, HideInInspector]
    public float fullIlluminousAngle = 30;

    [HideInInspector] public bool useShadowMap = false;
    
    public enum LightType
    {
        Directional = 0,
        Point = 1,
        Spot = 2
    }

    public LightType GetLightType()
    {
        return m_lightType;
    }

    public RTLightStructureDirectional_t GetDirectionalLight()
    {
        return new RTLightStructureDirectional_t()
        {
            color = color.ToVector3(),
            direction = -1 * Vector3.Normalize(transform.forward),
        };
    }

    public RTLightStructurePoint_t GetPointLight()
    {
        return new RTLightStructurePoint_t()
        {
            color = color.ToVector3(),
            position = transform.position,
        };
    }
    
    public RTLightStructureSpot_t GetSpotLight()
    {
        return new RTLightStructureSpot_t()
        {
            color = color.ToVector3(),
            coneAngle = coneAngle * Mathf.Deg2Rad,
            cosConeAngle = Mathf.Cos(coneAngle * Mathf.Deg2Rad / 2f),
            cosFullIlluminous = Mathf.Cos(fullIlluminousAngle * Mathf.Deg2Rad / 2f),
            direction = -1 * Vector3.Normalize(transform.forward),
            position = transform.position,
        };
    }
    
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "outline_highlight_white_24dp.png", true);
    }
    
    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}