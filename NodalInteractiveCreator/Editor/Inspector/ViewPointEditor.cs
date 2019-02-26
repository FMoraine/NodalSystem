using Machinika.Viewpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Machinika.Editor.CameraEditor
{
    [CustomEditor(typeof(ViewPoint))]
    public class ViewPointEditor : UnityEditor.Editor
    {
        private ViewPoint subject;
        private Camera c;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            subject = target as ViewPoint;

            if (GUILayout.Button("Set Camera Pos to Actual Pos") && c)
                subject.CameraPosition = c.transform.position;
        }

        void OnSceneGUI()
        {
            subject = target as ViewPoint;
            c = Camera.current;

           EditorGUI.BeginChangeCheck();

            Vector3 camPos = Handles.PositionHandle(subject.CameraPosition, subject.transform.rotation);
            Vector3 targetPos = Handles.PositionHandle(subject.TargetPosition, subject.transform.rotation);
     
            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject(target, "transform Modifications");
                subject.CameraPosition = camPos;
                subject.TargetPosition = targetPos;

            }
        }
    }
}
