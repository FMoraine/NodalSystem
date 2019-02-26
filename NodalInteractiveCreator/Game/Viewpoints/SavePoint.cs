// Machinika Museum Prototype v.01
// © Littlefield Studio
// Writted by Rémi Carreira - 2015
//
// SavePoint.cs

using UnityEngine;
using System.Collections;

namespace NodalInteractiveCreator.Viewpoints
{
    public class SavePoint
    {
        public ViewPoint ViewPoint = null;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;

        public SavePoint(ViewPoint Point, Vector3 CameraPosition, Quaternion CameraRotation)
        {
            ViewPoint = Point;
            Position = CameraPosition;
            Rotation = CameraRotation;
        }
    }
}