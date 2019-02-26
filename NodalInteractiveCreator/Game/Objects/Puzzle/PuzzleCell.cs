using System;
using System.Collections.Generic;
using Machinika.Tools;
using UnityEngine;

namespace NodalInteractiveCreator.Objects.Puzzle
{
    [Serializable]
    public class PuzzleCell
    {
        [SerializeField]
        private Quaternion _orientation;
        public Vector3 localPosition;
        public Vector3 localRotPos;
        public Vector3 worldPos;
        public Quaternion worldOrientation;
        public Vector2Integer gridPos;
        public List<ConnexionPath> paths;
        public int flag;
        public List<PuzzleObject> content;

        public PuzzleCell(List<PuzzleObject> pContent, Vector2Integer pGridPos)
        {
            flag = 0;
            gridPos = pGridPos;
            content = pContent;
            paths = new List<ConnexionPath>();
            Orientation = Quaternion.identity;
        }

        public void AddConnection(float pAngleMedian, float pangleSpread, Vector2Integer gridPos, BezierCurve curve, bool modifyByO = false)
        {
            if (modifyByO)
                curve.curvePoint1 = Orientation * curve.curvePoint1;

            paths.Add(new ConnexionPath(pAngleMedian, pangleSpread, gridPos.x, gridPos.y, curve));
        }

        public void SetOrientation(Quaternion newO, Quaternion parentRot)
        {
            Orientation = newO;

            foreach (var path in paths)
                path.bezierPath.curvePoint1 = Orientation * path.bezierPath.curvePoint1;

        }

        public Quaternion Orientation {

            get { return _orientation; }
            private set { _orientation = value; }
        }


    }

    [Serializable]
    public class ConnexionPath
    {
        public ConnexionPath(float pAngleMedian, float pangleSpread, int xGrid, int yGrid, BezierCurve curve)
        {
            angleSpread = pangleSpread;
            angleMedian = pAngleMedian;
            bezierPath = curve;
            gridPos = new Vector2Integer(xGrid, yGrid);
            destCell = null;
        }

        public float angleSpread;
        public float angleMedian;
        public BezierCurve bezierPath;
        public Vector2Integer gridPos;

        public PuzzleCell destCell { get; set; }
    }

    [Serializable]
    public struct BezierCurve
    {
        public BezierCurve(Vector3 p1, Vector3 c1, Vector3 p2, Vector3 c2)
        {
            point1 = p1;
            curvePoint1 = c1;

            point2 = p2;
            curvePoint2 = c2;
        }

        public static BezierCurve Linear { get { return new BezierCurve(); } }

        public BezierCurve(Vector3 tang1, Vector3 tang2)
        {
            curvePoint1 = tang1;
            curvePoint2 = tang2;
            point1 = Vector3.zero;
            point2 = Vector3.zero;
        }

        public Vector3 point1;
        public Vector3 curvePoint1;

        public Vector3 point2;
        public Vector3 curvePoint2;
    }



}
