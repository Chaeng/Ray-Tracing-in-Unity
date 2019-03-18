using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RTTexture_t
{
    public int id;

    // support for image
    public int isColor;
    public int imageIndex;

    // support for checker pattern
    public int isChecker;
    public int uRepeat, vRepeat;
    public Vector3 color1, color2;

    // functions
    public static int GetSize()
    {
        return sizeof(int) * 6
            + Vector3Extension.SizeOf() * 2;
    }
}
