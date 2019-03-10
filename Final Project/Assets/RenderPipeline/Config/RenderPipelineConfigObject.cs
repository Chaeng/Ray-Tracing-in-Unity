using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Render Pipeline Config")]
public class RenderPipelineConfigObject : ScriptableObject
{
    public Texture skybox;
    
    public Color upperAmbitent = Color.white;

    public Color lowerAmbitent = Color.white;

    public float globalRefractiveIndex = 1.0f;
    
    public int secondaryRayGeneration = 0;

    public bool enableShadow = false;

    public Color fogColor = Color.white;
    
    public int fogFactor = 0;


    private void Awake()
    {
        if (skybox == null)
        {
            skybox = new Texture2D(2, 2);
        }
    }
}
