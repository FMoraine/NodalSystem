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
[CustomNodalEditor(typeof(SetBlockItemTargetNode))]
public class SetBlockItemTargetNodeEditor : ActionNodeEditor<SetBlockItemTargetNode>
{
    private ItemTarget _itemTarget;
    public SetBlockItemTargetNodeEditor(Type t) : base(t) { }

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

        EditorGUILayout.LabelField("Object target", EditorStyles.centeredGreyMiniLabel);
        _itemTarget = EditorGUILayout.ObjectField(_node._target.Subject, typeof(ItemTarget), true) as ItemTarget;
        _node._target.Assign(_itemTarget);

        if (_itemTarget == null)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.Space();

        _node._value = PopupForLabelsBool(new GUIContent("Set value", ""), _node._value, "True (Block)", "False (Unblock)");

        EditorGUILayout.LabelField("Current value : " + _itemTarget._blockEnabled, EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        if (_itemTarget == null)
            return;

        NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
        NodeConnector.SetStyleAt(_node.actStart.MainLayer, "U", "Emit if Unblock", Color.green);
        NodeConnector.HiddenLayer(_node.actFinal.MainLayer, !_itemTarget._blockEnabled);
        if (_itemTarget._blockEnabled)
            NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "B", "Emit if Block", Color.red);

        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, false);
    }

}
#endif
