using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetActiveGONode))]
public class SetActiveGONodeEditor : ActionNodeEditor<SetActiveGONode>
{
    public SetActiveGONodeEditor(Type t) : base(t) { }

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

        EditorGUILayout.LabelField("GameObject target", EditorStyles.centeredGreyMiniLabel);
        _node._gameObject.Assign(EditorGUILayout.ObjectField(_node._gameObject, typeof(GameObject), true) as GameObject);

        if (_node._gameObject.Subject != null)
        {
            _node._value = PopupForLabelsBool(new GUIContent("Set value", ""), _node._value, null, null);
            EditorGUILayout.LabelField("Current value : " + _node._gameObject.Subject.activeSelf, EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }

}
#endif

