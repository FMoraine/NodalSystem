using System;
using System.Collections.Generic;
using Machinika.Tools;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    public class PuzzleMath
    {
        public static int GtoI(Vector2Integer gridPos, Vector2Integer gridSize)
        {
            if (gridPos.x >= gridSize.x || gridPos.x < 0)
                return -1;

            return Mathf.FloorToInt((float)gridPos.y * gridSize.x + gridPos.x);
        }
        public static int GtoI(int x , int y, Vector2Integer gridSize)
        {
            return Mathf.FloorToInt((float)y * gridSize.x + x);
        }
        public static Vector2Integer ItoG(int index, Vector2Integer gridSize)
        {
            return new Vector2Integer((int)(index % gridSize.x), Mathf.FloorToInt(index / gridSize.x));
        }
        public static Vector3 ItoW(int index, Vector2Integer gridSize, float tileSize)
        {
            return GtoW(ItoG(index, gridSize), gridSize, tileSize);
        }
        public static Vector3 GtoW(Vector2Integer gridPos, Vector2Integer gridSize, float tileSize)
        {
            Vector3 decal = new Vector3(gridSize.x * tileSize , gridSize.y * tileSize);
            Vector3 right = Vector3.right * ((tileSize * gridPos.x + tileSize / 2)- decal.x/2 );
            Vector3 forward = -Vector3.forward * ((tileSize * gridPos.y + tileSize / 2)- decal.y/2 ) ;
            return right + forward;
        }

        public static List<T> IntFlagTo<T>(int flag , T[] arrayTags)
        {
            List<T> selection = new List<T>();

            if (flag == -1)
            {
                selection.AddRange(arrayTags);
                return selection;
            }

            int arraySize = arrayTags.Length;
            
            for (int i = 0; i < arraySize; i++)
            {
                int mask = (int)Mathf.Pow(2, i);
                if (mask == (flag & mask))
                    selection.Add(arrayTags[i]);
            }
            return selection;
        }

        public static List<T> IntFlagTo<T>(int flag, List<T> arrayTags)
        {          
            return IntFlagTo(flag , arrayTags.ToArray());
        }

        public static List<PuzzleObject> FlagToObjects(int flag, int numberElements)
        {

            List<PuzzleObject> l = new List<PuzzleObject>();

            if (flag == -1)
            {
                for (int i = 0; i < numberElements; i++)
                    l.Add(new PuzzleObject(i));
                return l;
            }
            for (int i = 0; i < numberElements; i++)
            {
                int mask = (int)Mathf.Pow(2, i);
                if (mask == (flag & mask))
                    l.Add(new PuzzleObject(i));
            }

            return l;
        }

        public static float NormalizeDegree(float angle)
        {
            return angle % 360;
        }

        public static bool IsInAngleSpread(float current , float angleMedian, float spread)
        {       

            return Mathf.Abs(Mathf.DeltaAngle(current, angleMedian)) < spread/2;
        }

        public static bool IDIsInMask(int id , int flag)
        {
            int mask = (int)Mathf.Pow(2, id);
            return mask == (flag & mask) ;
        }

        public static List<int> FlagToIntA(int flag)
        {
            List<int> l = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                if (flag == (flag | (1 << i)))
                {
                    l.Add(i);
                }
            }

            return l;
        }

        public static Vector3 Casteljau(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float tPos)
        {
            
            var ap1 = Vector3.Lerp(p1, p2, tPos);
            var ap2 = Vector3.Lerp(p2, p3, tPos);
            var ap3 = Vector3.Lerp(p3, p4, tPos);

            var bp1 = Vector3.Lerp(ap1, ap2, tPos);
            var bp2 = Vector3.Lerp(ap2, ap3, tPos);

            return Vector3.Lerp(bp1, bp2, tPos);
        }

        public static double getClosestPointToCubicBezier(double fx, double fy, int slices, int iterations, double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return getClosestPointToCubicBezier(iterations, fx, fy, 0, 1d, slices, x0, y0, x1, y1, x2, y2, x3, y3);
        }

        private static double getClosestPointToCubicBezier(int iterations, double fx, double fy, double start, double end, int slices, double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3)
        {
            if (iterations <= 0) return (start + end) / 2;
            double tick = (end - start) / (double)slices;
            double x, y, dx, dy;
            double best = 0;
            double bestDistance = 9999;
            double t = start;

            while (t <= end)
            {
                x = (1 - t) * (1 - t) * (1 - t) * x0 + 3 * (1 - t) * (1 - t) * t * x1 + 3 * (1 - t) * t * t * x2 + t * t * t * x3;
                y = (1 - t) * (1 - t) * (1 - t) * y0 + 3 * (1 - t) * (1 - t) * t * y1 + 3 * (1 - t) * t * t * y2 + t * t * t * y3;

                dx = x - fx;
                dy = y - fy;
                dx *= dx;
                dy *= dy;
                var currentDistance = dx + dy;
                if (currentDistance < bestDistance)
                {
                    bestDistance = currentDistance;
                    best = t;
                }
                t += tick;
            }
            return getClosestPointToCubicBezier(iterations - 1, fx, fy, Math.Max(best - tick, 0d), Math.Min(best + tick, 1d), slices, x0, y0, x1, y1, x2, y2, x3, y3);
        }
    }
}
