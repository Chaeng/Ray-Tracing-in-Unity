using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelGridGizmos : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        int dimen = RayTracingRenderer.SceneParser.Instance.GetSpatialGridDimension();
        Vector3 MIN = RayTracingRenderer.SceneParser.Instance.GetSpatialGridBoxMin();
        Vector3 MAX = RayTracingRenderer.SceneParser.Instance.GetSpatialGridBoxMax();
        float xStep = (MAX.x - MIN.x) / dimen;
        float yStep = (MAX.y - MIN.y) / dimen;
        float zStep = (MAX.z - MIN.z) / dimen;

        for(int i = 0; i <= dimen; i++)
        {
            DrawZ(MIN, MAX, dimen, MIN.x + i * xStep, yStep);
            DrawY(MIN, MAX, dimen, MIN.z + i * zStep, xStep);
            DrawX(MIN, MAX, dimen, MIN.y + i * yStep, zStep);
        }
    }

    private void DrawZ(Vector3 MIN, Vector3 MAX, int dimen, float x, float yStep)
    {
        Vector3 head = new Vector3(x, MIN.y, MIN.z);
        Vector3 tail = new Vector3(x, MIN.y, MAX.z);

        for (int i = 0; i <= dimen; i++)
        {
            Gizmos.DrawLine(head, tail);

            head.y += yStep;
            tail.y += yStep;
        }
    }


    private void DrawY(Vector3 MIN, Vector3 MAX, int dimen, float z, float xStep)
    {
        Vector3 head = new Vector3(MIN.x, MIN.y, z);
        Vector3 tail = new Vector3(MIN.x, MAX.y, z);

        for (int i = 0; i <= dimen; i++)
        {
            Gizmos.DrawLine(head, tail);

            head.x += xStep;
            tail.x += xStep;
        }
    }


    private void DrawX(Vector3 MIN, Vector3 MAX, int dimen, float y, float zStep)
    {
        Vector3 head = new Vector3(MIN.x, y, MIN.z);
        Vector3 tail = new Vector3(MAX.x, y, MIN.z);

        for (int i = 0; i <= dimen; i++)
        {
            Gizmos.DrawLine(head, tail);

            head.z += zStep;
            tail.z += zStep;
        }
    }
}
