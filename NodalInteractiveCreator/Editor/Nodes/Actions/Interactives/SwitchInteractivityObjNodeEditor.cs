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
[CustomNodalEditor(typeof(SwitchInteractivityObjNode))]
public class SwitchInteractivityObjNodeEditor : ActionNodeEditor<SwitchInteractivityObjNode>
{
    public SwitchInteractivityObjNodeEditor(Type t) : base(t) { }

    public override Rect GetCustomBounds()
    {
        bounds.size = new Vector2(250, 140);
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
            if (_node._target.Subject.GetComponents<InteractiveObject>().Length > 1)
            {
                _node._isBetween = PopupForLabelsBool("Switch", _node._isBetween, "BETWEEN", "TO");

                List<string> labelsInteractiveObj = new List<string>();

                foreach (InteractiveObject io in _node._target.Subject.GetComponents<InteractiveObject>())
                    labelsInteractiveObj.Add(io.GetType().Name);

                if (_node._target.Subject.GetComponents<InteractiveObject>().Length > 2)
                    _node._idInteractiveObj1 = EditorGUILayout.Popup(_node._idInteractiveObj1, labelsInteractiveObj.ToArray());
                else
                {
                    _node._idInteractiveObj1 = 0;
                    EditorGUILayout.LabelField(_node._target.Subject.GetComponents<InteractiveObject>()[_node._idInteractiveObj1].GetType().Name, EditorStyles.miniButtonMid);
                }

                if (_node._isBetween)
                    EditorGUILayout.LabelField("⇑ ⇓", EditorStyles.centeredGreyMiniLabel);
                else
                    EditorGUILayout.LabelField("⇓ ⇓", EditorStyles.centeredGreyMiniLabel);

                if (_node._target.Subject.GetComponents<InteractiveObject>().Length > 2)
                    _node._idInteractiveObj2 = EditorGUILayout.Popup(_node._idInteractiveObj2, labelsInteractiveObj.ToArray());
                else
                {
                    _node._idInteractiveObj2 = 1;
                    EditorGUILayout.LabelField(_node._target.Subject.GetComponents<InteractiveObject>()[_node._idInteractiveObj2].GetType().Name, EditorStyles.miniButtonMid);
                }
            }
            else
                EditorGUILayout.HelpBox("This target have an interactiveObject script", MessageType.None);
        }
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
