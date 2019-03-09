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
                _light.coneAngle = EditorGUILayout.Slider("Cone Angle", _light.coneAngle, 0, 179);
                _light.penumbraAngle = EditorGUILayout.Slider("Penumbra Angle", _light.penumbraAngle, 0, 179);
                _light.useShadowMap = EditorGUILayout.Toggle("Use Shadow Map", _light.useShadowMap);
                break;
        }
    }
}
