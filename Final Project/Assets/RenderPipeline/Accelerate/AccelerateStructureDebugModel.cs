using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerateStructureDebugModel
{
    public static AccelerateStructureDebugModel Instance
    {
        get
        {
            if (_myInstance == null)
            {
                _myInstance = new AccelerateStructureDebugModel();
            }

            return _myInstance;
        }
    }

    private static AccelerateStructureDebugModel _myInstance;


    public Vector3 min = Vector3.zero;
    public Vector3 max = Vector3.zero;

    public void SetMaxMin(Vector3 max, Vector3 min)
    {
        this.max = max;
        this.min = min;
    }
}
