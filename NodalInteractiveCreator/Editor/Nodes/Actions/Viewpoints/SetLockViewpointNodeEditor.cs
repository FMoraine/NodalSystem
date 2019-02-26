using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using NodalInteractiveCreator.Viewpoints;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetLockViewpointNode))]
public class SetLockViewpointNodeEditor : ActionNodeEditor<SetLockViewpointNode>
{
    public SetLockViewpointNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 75);
        return bounds;
    }

    public override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void ShowOnNodeInit()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Viewpoint", EditorStyles.centeredGreyMiniLabel);
        _node._vp.Assign(EditorGUILayout.ObjectField(_node._vp.Subject, typeof(ViewPoint), true) as ViewPoint);

        if (_node._vp.Subject != null)
        {
            _node._value = PopupForLabelsBool("", _node._value, "Lock", "Unlock");
            EditorGUILayout.LabelField("Current value : " + _node._vp.Subject.LockViewpointOnStart, EditorStyles.centeredGreyMiniLabel);

        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }
}
#endif

