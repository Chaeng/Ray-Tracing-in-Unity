using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Vector3 ToVector3(this Color c)
    {
        return new Vector3(c.r, c.g, c.b);
    }
}
