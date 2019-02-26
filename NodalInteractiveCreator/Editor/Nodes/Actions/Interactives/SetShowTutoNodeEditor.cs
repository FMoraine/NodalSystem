using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomNodalEditor(typeof(SetShowTutoNode))]
public class SetShowTutoNodeEditor : ActionNodeEditor<SetShowTutoNode>
{
    public SetShowTutoNodeEditor(Type t) : base(t) { }

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

        _node._showTuto = PopupForLabelsBool("Tuto", _node._showTuto, "Show", "Unshow");

        if (!_node._showTuto)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField("Step", EditorStyles.centeredGreyMiniLabel);
        _node._step = (Tuto.TUTO_STEP)EditorGUILayout.EnumPopup(_node._step);

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        NodeConnector.HiddenLayer(_node.actStart.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actFinal.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, true);
    }
}
#endif
