 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.TheoricalNodes;
using UnityEditor;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing
{
    [CustomNodalEditor(typeof(DelayNode))]
    public class DelayNodeEditor : NodalEditorCustom
    {
        public DelayNodeEditor(Type subject) : base(subject)
        {
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUILayout.Button("Custom Editor Button");
          
        }

        public override Rect GetCustomBounds()
        {
            bounds.width = 200;
            bounds.height = 300;
            return bounds;
        }
    }
}
