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
[CustomNodalEditor(typeof(SetInteractableObjNode))]
public class SetInteractableObjNodeEditor : ActionNodeEditor<SetInteractableObjNode>
{
    public SetInteractableObjNodeEditor(Type t) : base(t) { }

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
        base.ShowOnNodeInit();
        if (_node._target.Subject == null)
            return;

        EditorGUILayout.BeginVertical();

        _node._value = PopupForLabelsBool(new GUIContent("Set value", ""), _node._value, "True (Active)", "False (Inactive)");

        EditorGUILayout.LabelField("Current value : " + _node._target.Subject._interactable, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }

}
#endif

