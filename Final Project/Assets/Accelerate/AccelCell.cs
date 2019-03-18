using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayTracingRenderer
{
    public class AccelCell
    {
        private Bounds _aabb;    // Axis aligned bounding box
        private List<RTTriangle_t> _triangles = null;
        
        public AccelCell()
        {
            _aabb = new Bounds();
            _triangles = new List<RTTriangle_t>();
        }


        public void SetMinMax(Vector3 min, Vector3 max)
        {
            _aabb.SetMinMax(min, max);
            _triangles.Clear();
        }


        public void AddTriangle(RTTriangle_t triangleT)
        {
            _triangles.Add(triangleT);
        }


        public List<RTTriangle_t> GetTriangles()
        {
            return _triangles;
        }


        public override string ToString()
        {
            return $"Center={_aabb.center} Size={_aabb.size} Triangles={_triangles.Count}";
        }
    }
}

