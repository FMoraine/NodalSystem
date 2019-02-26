using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.CustomEditing;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Game.TheoricalNodes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using NodalInteractiveCreator.Controllers;
using NodalInteractiveCreator.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomNodalEditor(typeof(ActiveActionNode))]
public class ActiveActionNodeEditor : ActionNodeEditor<ActiveActionNode>
{
    public ActiveActionNodeEditor(Type t) : base(t) { }

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
            InitLabelsEmitterNodeForPopup(out _labelsEmitter, out _listChecks);

            if (_node._idAction < _labelsEmitter.Count)
            {
                EditorGUILayout.LabelField("Node", EditorStyles.centeredGreyMiniLabel);
                _node._idAction = EditorGUILayout.Popup(_node._idAction, _labelsEmitter.ToArray());
            }
            else
                _node._idAction = 0;

            _node._tagAction = _labelsEmitter[_node._idAction];

            if (_node._tagAction == "Check")
                _node._idCheck = EditorGUILayout.Popup(_node._idCheck, _listChecks.ToArray());
        }
        else
        {
            _node._idAction = 0;
            _node._tagAction = "";
            _node._idCheck = 0;
        }

        EditorGUILayout.EndVertical();
    }

    protected override void ChangeNodeEmitterStyle()
    {
        base.ChangeNodeEmitterStyle();
    }

}
#endif

