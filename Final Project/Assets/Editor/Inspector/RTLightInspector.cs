using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RTLight))]
public class RTLightInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var _light = target as RTLight;
        
        DrawDefaultInspector();

        switch (_light.GetLightType())
        {
            case RTLight.LightType.Directional:

                break;
            
            case RTLight.LightType.Point:

                break;
            
            case RTLight.LightType.Spot:
                _light.coneAngle = EditorGUILayout.Slider("Cone angle", _light.coneAngle, 0, 179);
                _light.fullIlluminousAngle = EditorGUILayout.Slider("Full illumination angle", _light.fullIlluminousAngle, 0, 179);
                _light.penumbraDecay = EditorGUILayout.FloatField("Penumbra Decay", _light.penumbraDecay);
                _light.useShadowMap = EditorGUILayout.Toggle("Use shadow map", _light.useShadowMap);
                break;
        }
    }
}
