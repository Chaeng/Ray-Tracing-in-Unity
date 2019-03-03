using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Render Pipeline Config")]
public class RenderPipelineConfigObject : ScriptableObject
{
    public Texture skybox = new Texture2D(2, 2);
    
    public Color ambitent = Color.white;

    public int secondaryRayGeneration = 0;

    public bool enableShadow = false;

    
}
