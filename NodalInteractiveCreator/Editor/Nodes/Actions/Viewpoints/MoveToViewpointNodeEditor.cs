using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Viewpoints;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(MoveToViewpointNode))]
public class MoveToViewpointNodeEditor : ActionNodeEditor<MoveToViewpointNode>
{
    public MoveToViewpointNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 100);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Viewpoint target", EditorStyles.centeredGreyMiniLabel);
        _node._vp.Assign(EditorGUILayout.ObjectField(_node._vp.Subject, typeof(ViewPoint), true) as ViewPoint);

        if (_node._vp.Subject != null)
        {
            _node._idMoveTo = EditorGUILayout.Popup(_node._idMoveTo, new string[] { "ZoomTo", "ZoomOutTo" });

            switch (_node._idMoveTo)
            {
                case 0:
                    EditorGUILayout.HelpBox("Zoom to viewpoint target and save the current viewpoint as last.", MessageType.None);
                    break;

                case 1:
                    EditorGUILayout.HelpBox("Zoom to viewpoint target and save the default viewpoint as last.", MessageType.None);
                    break;
            }
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }
}
#endif

