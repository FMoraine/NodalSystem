using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodalInteractiveCreator.Objects.Puzzle;
using NodalInteractiveCreator.Objects;
using UnityEngine;

namespace NodalInteractiveCreator.Endoscop
{
    [Serializable]
    public class EndoscopeData
    {
        public List<PathPoint> path;
       
        public EndoscopeMovement movementMode = EndoscopeMovement.SNAPPING;
        public float snapDelta = 0.2f;
        public float speed;
        public float noisePower = 0.2f;
        public float noiseSpeed = 1.5f;
        public float rotationSpeed = 2f;
        public float maxAngleSpread = 160f;
    }

    [Serializable]
    public class PathPoint
    {
        public BezierCurve curve;
        public bool isCheckPoint;
        public int callMax;
        public int callCount;
        public Action actions;
    }

    public enum EndoscopeMovement
    {
        SNAPPING,
        LINEAR
    }
}
