using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayTracingRenderer
{
    public class AccelGridGroup
    {
        private const int DIMENSION = 3;


        private AccelCell[,,] _grids;
        private Vector3 _min = Vector3.zero;
        private Vector3 _max = Vector3.zero;


        /// <summary>
        /// This will create a grid array, from min (lower left front) to max (upper right back)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public AccelGridGroup()
        {
            _grids = new AccelCell[DIMENSION, DIMENSION, DIMENSION];

            for (int x = 0; x < DIMENSION; x++)
            {
                for (int y = 0; y < DIMENSION; y++)
                {
                    for (int z = 0; z < DIMENSION; z++)
                    {
                        _grids[x, y, z] = new AccelCell();
                    }
                }
            }
        }


        public void UpdateAccelGridGroup(Vector3 min, Vector3 max)
        {
            _min = min;
            _max = max;

            Vector3 size = CalculateGridSize(min, max);
            Vector3 delta = Vector3.zero;

            for (int x = 0; x < DIMENSION; x++)
            {
                for (int y = 0; y < DIMENSION; y++)
                {
                    for (int z = 0; z < DIMENSION; z++)
                    {
                        delta.x = x * size.x;
                        delta.y = y * size.y;
                        delta.z = z * size.z;
                        _grids[x, y, z].SetMinMax(min + delta, min + delta + size);
                    }
                }
            }
        }


        public void AddTriangle(RTTriangle_t triangleT)
        {
            var index0 = GetGridForVertex(triangleT.vert0);
            var index1 = GetGridForVertex(triangleT.vert1);
            var index2 = GetGridForVertex(triangleT.vert2);

            var min = (x: 0, y: 0, z: 0);
            min.x = Mathf.Min(index0.x, index1.x, index2.x);
            min.y = Mathf.Min(index0.y, index1.y, index2.y);
            min.z = Mathf.Min(index0.z, index1.z, index2.z);

            var max = (x: 0, y: 0, z: 0);
            max.x = Mathf.Max(index0.x, index1.x, index2.x);
            max.y = Mathf.Max(index0.y, index1.y, index2.y);
            max.z = Mathf.Max(index0.z, index1.z, index2.z);

            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    for (int z = min.z; z <= max.z; z++)
                    {
                        _grids[x, y, z].AddTriangle(triangleT);
                    }
                }
            }
        }


        private Vector3 CalculateGridSize(Vector3 min, Vector3 max)
        {
            Vector3 size = Vector3.zero;

            size.x = (max.x - min.x) / DIMENSION;
            size.y = (max.y - min.y) / DIMENSION;
            size.z = (max.z - min.z) / DIMENSION;

            return size;
        }


        private (int x, int y, int z) GetGridForVertex(Vector3 vertex)
        {
            var index = (x: 0, y: 0, z: 0);

            index.x = Mathf.FloorToInt((vertex.x - _min.x) / (_max.x - _min.x) * DIMENSION);
            index.x = Mathf.Clamp(index.x, 0, DIMENSION - 1);

            index.y = Mathf.FloorToInt((vertex.y - _min.y) / (_max.y - _min.y) * DIMENSION);
            index.y = Mathf.Clamp(index.y, 0, DIMENSION - 1);

            index.z = Mathf.FloorToInt((vertex.z - _min.z) / (_max.z - _min.z) * DIMENSION);
            index.z = Mathf.Clamp(index.z, 0, DIMENSION - 1);

            return index;
        }
    }
}