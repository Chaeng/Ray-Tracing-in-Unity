using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RTTexture_t
{
    public int id;

    // support for checker
    public int isChecker;
    public int uRepeat, vRepeat;
    public Vector3 color1, color2;

    // functions
    public static int GetSize()
    {
        return sizeof(int) * 4
            + Vector3Extension.SizeOf() * 2;
    }
}
