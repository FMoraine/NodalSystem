using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodalInteractiveCreator.Objects.Puzzle;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
    public class EndoscopeMath
    {
        public static void ReverseNormals(MeshFilter meshF)
        {
            if (meshF != null)
            {
                Mesh mesh = meshF.mesh;

                Vector3[] normals = mesh.normals;
                for (int i = 0; i < normals.Length; i++)
                    normals[i] = -normals[i];
                mesh.normals = normals;

                for (int m = 0; m < mesh.subMeshCount; m++)
                {
                    int[] triangles = mesh.GetTriangles(m);
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        int temp = triangles[i + 0];
                        triangles[i + 0] = triangles[i + 1];
                        triangles[i + 1] = temp;
                    }
                    mesh.SetTriangles(triangles, m);
                }
            }
        }

        public static Vector3 LerpAlongBezier(BezierCurve curve1, BezierCurve curve2, float progress)
        {
            Vector3 from = curve1.point1;
            Vector3 to = curve2.point1;

            return PuzzleMath.Casteljau(from, from + curve1.curvePoint1,
                to - curve2.curvePoint1, to, progress);
        }

        public static float GetLengthOfBezier(BezierCurve curve, BezierCurve curve2, int segCounts = 10)
        {
            float dist = 0;
            Vector3[] segs = new Vector3[segCounts];
            for (int i = 0; i < segCounts; i++)
            {
                segs[i] = LerpAlongBezier(curve, curve2, (float)i / (segCounts - 1));
                if(i > 0)
                    dist += Vector3.Distance(segs[i-1], segs[i]);
            }
          
            return dist;
        }
    }
}
