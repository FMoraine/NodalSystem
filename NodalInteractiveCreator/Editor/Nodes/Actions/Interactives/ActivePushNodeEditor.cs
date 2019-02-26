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
[CustomNodalEditor(typeof(ActivePushNode))]
public class ActivePushNodeEditor : ActionNodeEditor<ActivePushNode>
{
    private PushObject _pushObject;
    public ActivePushNodeEditor(Type t) : base(t) { }

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
        _pushObject = EditorGUILayout.ObjectField(_node._target.Subject, typeof(PushObject), true) as PushObject;
        _node._target.Assign(_pushObject);

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        if (_pushObject == null)
            return;

        NodeConnector.SetStyleAt(_node.actOnActive.MainLayer, "E", "Called when the push is actived", Color.magenta);
        NodeConnector.HiddenLayer(_node.actStart.MainLayer, false);
        NodeConnector.HiddenLayer(_node.actProgress.MainLayer, false);
        NodeConnector.HiddenLayer(_node.actCheck.MainLayer, true);
        NodeConnector.HiddenLayer(_node.actTouch.MainLayer, _pushObject._interactable);

        if (_pushObject._twoStep)
        {
            NodeConnector.HiddenLayer(_node.actFinal.MainLayer, false);
            NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "F", "Emit if on final", Color.red);
        }
        else
        {
            NodeConnector.HiddenLayer(_node.actFinal.MainLayer, !_pushObject._onPress);

            if (_pushObject._onPress)
                NodeConnector.SetStyleAt(_node.actFinal.MainLayer, "P", "Emit if pressed", Color.red);
        }
    }

}
#endif
