using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Objects;
using NodalInteractiveCreator.Tools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetActiveBackToStartNode))]
public class SetActiveBackToStartNodeEditor : ActionNodeEditor<SetActiveBackToStartNode>
{
    private TranslatableObject _translatableObject;
    private RotatableObject _rotatableObject;

    public SetActiveBackToStartNodeEditor(Type t) : base(t) { }

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

        EditorGUILayout.BeginVertical();

        if (_node._target.Subject != null)
        {
            if (_node._target.Subject is TranslatableObject || _node._target.Subject is RotatableObject)
                _node._value = PopupForLabelsBool(new GUIContent("Set Value :", ""), _node._value, "", "");

            if (_node._target.Subject is TranslatableObject)
            {
                _translatableObject = _node._target.Subject as TranslatableObject;
                EditorGUILayout.LabelField("Current value : " + _translatableObject._backToStart.ToString(), EditorStyles.centeredGreyMiniLabel);
            }
            else if (_node._target.Subject is RotatableObject)
            {
                _rotatableObject = _node._target.Subject as RotatableObject;
                EditorGUILayout.LabelField("Current value : " + _rotatableObject._backToStart.ToString(), EditorStyles.centeredGreyMiniLabel);
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

